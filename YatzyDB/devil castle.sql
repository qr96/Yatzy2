create table devil_castle_info (
user_id			varchar(20) primary key,
opened 			bool not null,
now_level		int not null,
max_level		int not null
) engine=MyISAM charset=utf8;

/* 악마성 정보 존재 여부 확인 */
select exists (select user_id from devil_castle_info where user_id='뿡뿡이');

/* 악마성 정보 추가 */
insert into devil_castle_info values('10005', false, 0, 0);

/* 악마성 정보 제거 */
delete from devil_castle_info where user_id='뿡뿡이';

/* 악마성 정보 가져오기 */
select opened, now_level, max_level from devil_castle_info where user_id='뿡뿡이';

/* 악마성 열기 */
update devil_castle_info set opened = true where user_id='뿡뿡이';

/* 악마성 승리 */
update devil_castle_info set now_level=now_level+1, max_level=(case when now_level>max_level then now_level else max_level end) where user_id='뿡뿡이4';

/* 악마성 패배 */
update devil_castle_info set opened=false, now_level=0 where user_id='뿡뿡이';

/* 유저 상금 */
update user_info set money=money+1000*pow(2, (select now_level from devil_castle_info where user_id='뿡뿡이')) where user_id='뿡뿡이';

/* 유저 순위 (100위까지) */
select user_info.nickname, max_level
from devil_castle_info, user_info
where devil_castle_info.user_id = user_info.user_id
order by max_level desc
limit 100;

select * from user_info;
select * from devil_castle_info;

