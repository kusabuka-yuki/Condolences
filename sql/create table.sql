CREATE TABLE Deceaseds(
	area varchar(255) , 
	name varchar(255) , 
	age int , 
	reason text, 
	description text ,
	published_at varchar(255) , 
	created_at datetime default '1000-01-01 00:00:00'
);