CREATE TABLE Deceaseds(
	area varchar(255) , 
	name varchar(255) , 
	age int , 
	reason text, 
	description text ,
	published_at varchar(255) , 
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);