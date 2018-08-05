CREATE DATABASE IMDB
Go


CREATE SCHEMA imdb_schema
Go


USE IMDB
Go




CREATE TABLE IMDB.dbo.Actors(
actor_id uniqueidentifier default newid() Primary key,
Name Varchar(100) not null,
Sex Varchar(15),
DOB date default '0000-00-00',
Bio Varchar(255));
Go

CREATE TABLE IMDB.dbo.Producers(
producer_id uniqueidentifier default newid() Primary key,
Name Varchar(100) not null,
Sex Varchar(15),
DOB date default '0000-00-00',
Bio Varchar(255));
Go

CREATE TABLE IMDB.dbo.Movie(
movie_id uniqueidentifier default newid() Primary Key,
Name varchar(100),
Release int check(Release>1913),
Poster image,
Plot varchar(255));
Go


CREATE TABLE IMDB.dbo.Mapping_Table_Actors_and_movies(
movie_id uniqueidentifier default newid(),
actor_id uniqueidentifier default newid(),
Primary key(movie_id,actor_id));

CREATE TABLE IMDB.dbo.Mapping_Table_Producers_and_Movies(
movie_id uniqueidentifier default newid() unique,
producer_id uniqueidentifier default newid(),
Primary key(movie_id,producer_id));




Insert into IMDB.dbo.Actors(actor_id,Name,Sex,DOB,Bio)
 values(default,'Robert De Niro','Male','1957-06-08','De Niros longtime collaboration with director Martin Scorsese earned him the Academy Award for Best Actor for his portrayal of Jake LaMotta in the 1980 film Raging Bull.'),
 (default,'Al Pacino','Male','1960-10-30','Pacino received his first Best Actor Oscar nomination for Serpico (1973). he was also nominated for The Godfather Part II, Dog Day Afternoon (1975)');
 (default,'Leonardo Dicaprio','Male','1975-11-11','DiCaprio began his career by appearing in television commercials in the late 1980s. He next had recurring roles in various television series, such as the soap opera Santa Barbara and the sitcom Growing Pains.'),

select * from IMDB.dbo.Actors;




Insert into IMDB.dbo.Producers(producer_id,Name,Sex,DOB,Bio)
 values(default,'Matt Demon','Male','1960-10-30','Along with Ben Affleck and producers Chris Moore and Sean Bailey, Damon founded the production company LivePlanet, through which the four created the Emmy-nominated documentary series Project Greenlight'),
 (default,'Brad Pitt','Male','1975-11-11','Brad Pitt produced The Departed (2006) and 12 Years a Slave (2013), both of which won the Academy Award for Best Picture, and also The Tree of Life, Moneyball, and The Big Short (2015), all of which garnered Best Picture nominations.');
 (default,'Steven Spielberg','Male','1957-06-08','Spielberg has increased his role as a film producer. He headed up the production team for several cartoons, including the Warner Bros. hits Tiny Toon Adventures, Animaniacs, Pinky and the Brain'),
 

select * from IMDB.dbo.Producers;




Insert into IMDB.dbo.Movie(movie_id,Name,Release,Plot)
values(default,'The Godfather-II',1975,'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.'),
(default,'The Godfather-III',1990,'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.');
(default,'The Godfather-I',1973,'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.');

select * from IMDB.dbo.Movie;




Insert into IMDB.dbo.Mapping_Table_Producers_and_Movies(movie_id,producer_id)
values
('9ACBAAB9-CD47-4101-ACA9-BBA48F70DC8F','90293332-0373-47A4-BC76-80584B9AFACD'),
('DA6382BC-0863-4EEC-B65C-E6FD31F507D0','A97FCCA1-A809-4B89-A777-895E3F0C7753'),
('14E2E00E-79DD-46D3-AA2D-75DDCC7AE074','2844B3BC-2E7F-4BE2-94BE-0E8F2D59FB6A');

select * from IMDB.dbo.Mapping_Table_Producers_and_Movies;




Insert into IMDB.dbo.Mapping_Table_Actors_and_movies(movie_id,actor_id)
values
('9ACBAAB9-CD47-4101-ACA9-BBA48F70DC8F','DB4FA7C6-4737-488E-9550-1DA4E4234382'),
('9ACBAAB9-CD47-4101-ACA9-BBA48F70DC8F','BEAEB4E6-D8C3-43BF-ADAD-70B90E55B36B'),
('DA6382BC-0863-4EEC-B65C-E6FD31F507D0','A0606F3A-E05D-4229-9917-F2DCEE9AC026'),
('DA6382BC-0863-4EEC-B65C-E6FD31F507D0','BEAEB4E6-D8C3-43BF-ADAD-70B90E55B36B'),
('14E2E00E-79DD-46D3-AA2D-75DDCC7AE074','DB4FA7C6-4737-488E-9550-1DA4E4234382'),
('14E2E00E-79DD-46D3-AA2D-75DDCC7AE074','A0606F3A-E05D-4229-9917-F2DCEE9AC026');


select * from IMDB.dbo.Mapping_Table_Actors_and_movies;




CREATE PROCEDURE imdb_schema.USP_MovieDetails_Delete @Movie_ID uniqueidentifier as
BEGIN
IF (@Movie_ID is not null)
BEGIN
Delete from IMDB.dbo.Movie where movie_id=@Movie_ID;
Delete from IMDB.dbo.Mapping_Table_Actors_and_movies where movie_id=@Movie_ID;
Delete from IMDB.dbo.Mapping_Table_Producers_and_Movies where movie_id=@Movie_ID;
END
END


