
select *
from user_info;

/*
set @max_nickname_seq =
(select max(nickname_seq)
from user_info
where nickname='뿡뿡이');

select @max_nickname_seq + 1;

*/
