create table user_info (
user_id			varchar(20) primary key,
nickname		varchar(20),
nickname_seq	int not null,
money			long not null,
ruby			long not null
) engine=MyISAM charset=utf8;