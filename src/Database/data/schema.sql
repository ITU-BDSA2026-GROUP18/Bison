drop table if exists proposal;
drop table if exists comment;
drop table if exists taxon;
drop table if exists observation;
drop table if exists user;

create table user (
  user_id integer primary key autoincrement,
  username string not null,
  email string not null,
  pw_hash string not null
);

create table post (
  post_id integer primary key autoincrement,
  author_id integer not null references user(user_id) on delete cascade,
  text text not null,
  pub_date integer not null
);

create table observation (
  post_id integer primary key references post(post_id) on delete cascade,
  taxon_id integer not null references taxon(taxon_id)
);

create table comment (
  post_id integer primary key references post(post_id) on delete cascade,
  observation_id integer not null references observation(post_id) on delete cascade
);

create table taxon (
  taxon_id integer primary key autoincrement,
  parent_taxon_id integer references taxon(taxon_id), -- intended to be nullable
  dwc_taxon_id string not null,
  vernacular_name string -- intended to be nullable
);

create table proposal (
  post_id integer primary key references post(post_id) on delete cascade,
  observation_id integer not null references observation(post_id) on delete cascade,
  taxon_id integer not null references taxon(taxon_id)
);