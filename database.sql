create database done_doluong
go

create table doluong2
(
	id int primary key,
	date_time nvarchar(50),
	so_luong nvarchar(1000)

)
go

insert into doluong2
(
	id, date_time, so_luong
)
values
(
	'1',N'2020/008/15', N'15'
)


select*from doluong2