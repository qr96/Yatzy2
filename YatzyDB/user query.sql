create table user_info (
user_id			varchar(20) primary key,
nickname		varchar(20),
nickname_seq	int not null,
money			long not null,
ruby			long not null
) engine=MyISAM charset=utf8;

/* 새로운 유저 추가 */
insert into user_info values('10005', '방방방구쟁이', 0, 10000, 0);

/* 존재 여부 확인 */
select exists (select user_id from user_info where nickname='뿡뿡이') as is_exists;

/* 유저 정보 가져오기 */
select nickname, nickname_seq, money, ruby from user_info where user_id='뿡뿡이';

/* 유저 돈 사용 */
update user_info set money=money-1000 where user_id='뿡뿡이';

/* 유저 돈 부여 */
update user_info set money=money+1000 where user_id='뿡뿡이';

select nickname, nickname_seq, money, ruby from user_info where user_id='빵방이';

select * from user_info;


