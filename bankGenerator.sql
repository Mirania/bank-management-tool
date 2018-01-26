use bank;

GO

CREATE TABLE USER_TYPE (
utype	int UNIQUE NOT NULL,
descr	varchar(10) NOT NULL,
PRIMARY KEY(utype),
);

CREATE TABLE EMPLOYEES (
id		int UNIQUE NOT NULL,
name	varchar(40) NOT NULL,
addr	varchar(50) NOT NULL,
postal	varchar(15) NOT NULL,
citid	varchar(15) NOT NULL,
gender	varchar(15) NOT NULL,
salary  decimal(6,2) NOT NULL,
usern	varchar(20) UNIQUE NOT NULL,
pword	varbinary(256) NOT NULL,
utype	int NOT NULL,
PRIMARY KEY(id),
FOREIGN KEY(utype) REFERENCES USER_TYPE(utype),
);

CREATE TABLE CLIENTS (
id		int UNIQUE NOT NULL,
name	varchar(40) NOT NULL,
addr	varchar(50) NOT NULL,
postal	varchar(15) NOT NULL,
citid	varchar(15) NOT NULL,
gender	varchar(15) NOT NULL,
nif		int UNIQUE NOT NULL,
PRIMARY KEY(id),
);

CREATE TABLE ACCOUNT_TYPE (
atype		int UNIQUE NOT NULL,
descr		varchar(20) NOT NULL,
interest	decimal(6,3) NOT NULL,
PRIMARY KEY(atype),
);

CREATE TABLE ACCOUNTS (
id		int UNIQUE NOT NULL,
balance	decimal(11,2) NOT NULL,
atype	int NOT NULL,
PRIMARY KEY(id),
FOREIGN KEY(atype) REFERENCES ACCOUNT_TYPE(atype),
);

CREATE TABLE CLIENT_ACCOUNTS (
aid		int NOT NULL,
cid		int NOT NULL,
PRIMARY KEY(aid, cid),
FOREIGN KEY(aid) REFERENCES ACCOUNTS(id),
FOREIGN KEY(cid) REFERENCES CLIENTS(id),
);

CREATE TABLE LOAN_TYPE (
ltype		int UNIQUE NOT NULL,
descr		varchar(30) NOT NULL,
interest	decimal(6,3) NOT NULL,
PRIMARY KEY(ltype),
);

CREATE TABLE LOANS (
id		int UNIQUE NOT NULL,
cid		int NOT NULL,
aid		int NOT NULL,
objval	decimal(9,2) NOT NULL,
reqval	decimal(9,2) NOT NULL,
ltype	int NOT NULL,
months	int NOT NULL,
appr	varchar(3) NOT NULL		CHECK (appr='yes' OR appr='no'),
PRIMARY KEY(id),
FOREIGN KEY(aid) REFERENCES ACCOUNTS(id),
FOREIGN KEY(cid) REFERENCES CLIENTS(id),
FOREIGN KEY(ltype) REFERENCES LOAN_TYPE(ltype),
);

CREATE TABLE LOAN_FINAL_INTEREST (
lid		int UNIQUE NOT NULL,
value	decimal(9,2) NOT NULL,
PRIMARY KEY(lid),
FOREIGN KEY(lid) REFERENCES LOANS(id),
);

CREATE TABLE ACC_FINAL_INTEREST (
aid		int UNIQUE NOT NULL,
value	decimal(9,2) NOT NULL,
PRIMARY KEY(aid),
FOREIGN KEY(aid) REFERENCES ACCOUNTS(id),
);

CREATE TABLE SHARES (
cid		int UNIQUE NOT NULL,
amt		int NOT NULL,
PRIMARY KEY(cid),
FOREIGN KEY(cid) REFERENCES CLIENTS(id),
);

--procedures

GO

CREATE PROC InsertAdmin @id int, @name varchar(40), @addr varchar(50), @postal varchar(15), 
@citid varchar(15), @gender varchar(15), @salary decimal(6,2), @usern varchar(20), @pword varchar(40)  AS
	INSERT INTO EMPLOYEES (id, name, addr, postal, citid, gender, salary, usern, pword, utype) VALUES
	(@id, @name, @addr, @postal, @citid, @gender, @salary, @usern, ENCRYPTBYPASSPHRASE('abcxyz', @pword), 1);

GO

CREATE PROC InsertEmployee @id int, @name varchar(40), @addr varchar(50), @postal varchar(15), 
@citid varchar(15), @gender varchar(15), @salary decimal(6,2), @usern varchar(20), @pword varchar(40)  AS
	INSERT INTO EMPLOYEES (id, name, addr, postal, citid, gender, salary, usern, pword, utype) VALUES
	(@id, @name, @addr, @postal, @citid, @gender, @salary, @usern, ENCRYPTBYPASSPHRASE('abcxyz', @pword), 0);

GO

CREATE PROC InsertAccount @id int, @balance decimal(11,2), @atype int, @value decimal(9,2) AS
	INSERT INTO ACCOUNTS (id, balance, atype) VALUES
	(@id, @balance, @atype);
	INSERT INTO ACC_FINAL_INTEREST (aid, value) VALUES
	(@id, @value);

GO

CREATE PROC GetLoginInfo @username varchar(20) AS
	Select usern, CAST(DECRYPTBYPASSPHRASE('abcxyz',pword) AS varchar(40)) AS pword, descr FROM 
	EMPLOYEES JOIN user_type ON employees.utype = user_type.utype where usern=@username;

GO

CREATE PROC DeleteAccount @id int AS
	DELETE FROM ACC_FINAL_INTEREST WHERE aid=@id;
	DELETE FROM CLIENT_ACCOUNTS WHERE aid=@id;
	DELETE FROM LOANS WHERE aid=@id;
	DELETE FROM ACCOUNTS WHERE id=@id;

GO

CREATE PROC DeleteClient @id int AS
	DELETE FROM CLIENT_ACCOUNTS WHERE cid=@id;
	DELETE FROM LOANS WHERE cid=@id;
	DELETE FROM SHARES WHERE cid=@id;
	DELETE FROM CLIENTS WHERE id=@id;

GO

CREATE PROC DeleteLoan @id int AS
	DELETE FROM LOAN_FINAL_INTEREST WHERE lid=@id;
	DELETE FROM LOANS WHERE id=@id;

GO

CREATE PROC EmployeeInsertLoan @id int, @cid int, @aid int, @objval decimal(9,2), 
@reqval decimal(9,2), @ltype int, @months int, @mv decimal(9,2) AS
	INSERT INTO LOANS (id, cid, aid, objval, reqval, ltype, months, appr) VALUES
	(@id, @cid, @aid, @objval, @reqval, @ltype, @months, 'no');
	INSERT INTO LOAN_FINAL_INTEREST (lid, value) VALUES
	(@id, @mv);

GO

CREATE PROC ManagerInsertLoan @id int, @cid int, @aid int, @objval decimal(9,2), 
@reqval decimal(9,2), @ltype int, @months int, @mv decimal(9,2) AS
	INSERT INTO LOANS (id, cid, aid, objval, reqval, ltype, months, appr) VALUES
	(@id, @cid, @aid, @objval, @reqval, @ltype, @months, 'yes');
	INSERT INTO LOAN_FINAL_INTEREST (lid, value) VALUES
	(@id, @mv);

GO

CREATE PROC TimeTravel AS
	DECLARE @id int, @balance decimal(11,2), @accvalue decimal(9,2),
	@aid int, @lid int, @months int, @value decimal(9,2);

	DECLARE C CURSOR FAST_FORWARD FOR
	select id, balance, value from accounts join ACC_FINAL_INTEREST on id=aid;
	OPEN C;

	FETCH C INTO @id, @balance, @accvalue;

	--update balances
	WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @balance = @balance + @accvalue;
			UPDATE ACCOUNTS SET balance=@balance WHERE id=@id;
			FETCH C INTO @id, @balance, @accvalue;
		END;

	CLOSE C;
	DEALLOCATE C;

	DECLARE D CURSOR FAST_FORWARD FOR
	select accounts.id, balance, loans.id, months, value from 
	(accounts join loans on loans.aid=accounts.id) join loan_final_interest on lid=loans.id where appr='yes';
	OPEN D;

	FETCH D INTO @aid, @balance, @lid, @months, @value;

	--pay monthly value
	WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @balance = @balance - @value;
			SET @months = @months - 1;
			UPDATE ACCOUNTS SET balance=@balance WHERE id=@aid;

			--delete loans with 0 months remaining
			IF @months=0
				EXEC DeleteLoan @lid;
			ELSE
				UPDATE LOANS SET months=@months WHERE id=@lid;

			FETCH D INTO @aid, @balance, @lid, @months, @value;
		END;

	CLOSE D;
	DEALLOCATE D;

GO

--fill tables

INSERT INTO USER_TYPE (utype, descr) VALUES
(0, 'employee'), (1, 'admin');

INSERT INTO CLIENTS (id, name, addr, postal, citid, gender, nif) VALUES
(1, 'Miranda Iris', 'Aveiro Sul Rua 17', '134-340', '430980439', 'Female', 4095809),
(2, 'Ernesto Rodrigues', 'Lisboa Centro Rua 21', '5765-340', '35434211', 'Male', 938479),
(3, 'Guilherme Erto', 'Vila Nova Rua 10', '166-31', '24234324', 'Male', 289378),
(4, 'Ulsa Lidia', 'Coimbra Sul Rua 11', '1366-345', '9243584', 'Female', 928798),
(5, 'Moniro Invicto', 'Cantanhede Rua 16', '3040-303', '19472474', 'Other', 129347),
(6, 'Inês Artur', 'Arazede Rua 18', '4101-234', '19478447', 'Female', 32394903);

INSERT INTO ACCOUNT_TYPE (atype, descr, interest) VALUES
(0, 'Ordem', 0),
(1, 'Prazo 6M', 0.02),
(2, 'Prazo 1A', 0.025),
(3, 'Prazo 3A', 0.03);

EXEC InsertAccount 1, 1017.25, 0, 0;
EXEC InsertAccount 2, 52.01, 0, 0;
EXEC InsertAccount 3, 16600.10, 3, 510.25;
EXEC InsertAccount 4, 14099.15, 2, 400.27;
EXEC InsertAccount 5, 4400.00, 1, 90.35;
EXEC InsertAccount 6, 22029.06, 3, 665.66;
EXEC InsertAccount 7, 110.65, 1, 3.45;
EXEC InsertAccount 8, 125.89, 2, 4.22;
EXEC InsertAccount 9, 8.91, 0, 0;

INSERT INTO CLIENT_ACCOUNTS (aid, cid) VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 2),
(3, 6),
(4, 6),
(5, 5),
(6, 4),
(7, 4),
(7, 3),
(8, 3),
(9, 3);

INSERT INTO LOAN_TYPE (ltype, descr, interest) VALUES
(0, '80 a 100 H', 2.5),
(1, '60 a 80 H', 2.25),
(2, '40 a 60 H', 2),
(3, '0 a 40 H', 1.8),
(4, '80 a 100 P', 5),
(5, '60 a 80 P', 4),
(6, '40 a 60 P', 3),
(7, '0 a 40 P', 2.5);

INSERT INTO LOANS (id, cid, aid, objval, reqval, ltype, months, appr) values
(1, 2, 2, 100000, 80000, 0, 10, 'no'),
(2, 4, 6, 125000, 83000, 1, 12, 'no'),
(3, 5, 5, 150000, 125300, 4, 10, 'yes'),
(4, 4, 7, 37000, 22000, 6, 10, 'no');

insert into LOAN_FINAL_INTEREST (lid, value) values
(1, 3700.25),
(2, 3840.43),
(3, 4264.35),
(4, 1225.66);

INSERT INTO SHARES (cid, amt) VALUES
(1, 1750),
(3, 2250),
(5, 15025),
(6, 13330);

EXEC InsertAdmin 1, 'Miguel Matias', 'Coimbra Rua 26', '3240-213', 6544232, 'Male', 2450.64, 'admin', 'admin';
EXEC InsertEmployee 2, 'Ana Sousa', 'Coimbra Rua 23', '3140-210', 2309480, 'Female', 1350.25, '001', '100';
EXEC InsertEmployee 3, 'Joana Silva', 'Aveiro Rua 1', '3654-234', 12423546, 'Female', 1644.43, '002', '200';
EXEC InsertEmployee 4, 'Marques Jesus', 'Lisboa Sul Rua 21', '234-5345', 2545644, 'Male', 1764.41, '003', '300';
EXEC InsertEmployee 5, 'Ernealda Asarias', 'Lisboa Centro Rua 77', '2347-5454', 96434543, 'Other', 1525.75, '004', '400';

GO

--indexes

CREATE INDEX IxClient ON 
CLIENTS(nif) 
INCLUDE (id, name)
WITH (FILLFACTOR = 85);

CREATE INDEX IxEmployee ON 
EMPLOYEES(id) 
INCLUDE (name, salary)
WITH (FILLFACTOR = 85);

CREATE INDEX IxLoanApproved ON 
LOANS(id) 
INCLUDE (cid, reqval, ltype, appr)
WHERE appr='yes'
WITH (FILLFACTOR = 85);

CREATE INDEX IxLoanPending ON 
LOANS(id) 
INCLUDE (cid, reqval, ltype, appr)
WHERE appr='no'
WITH (FILLFACTOR = 65);

GO