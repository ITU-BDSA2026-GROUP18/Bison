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

create table observation (
  observation_id integer primary key autoincrement,
  author_id integer not null,
  taxon_id integer not null references taxon(taxon_id),
  text string not null,
  pub_date integer
);

create table comment (
  comment_id integer primary key autoincrement,
  observation_id integer not null references observation(observation_id) on delete cascade,
  author_id integer not null references user(user_id),
  comment string not null,
  pub_date integer
);

create table taxon (
  taxon_id integer primary key autoincrement,
  parent_taxon_id integer references taxon(taxon_id), -- intended to be nullable
  dwc_taxon_id string not null,
  vernacular_name string -- intended to be nullable
);

create table proposal (
  proposal_id integer primary key autoincrement,
  author_id integer not null references user(user_id) on delete cascade, -- a proposal is always tied to a user
  observation_id integer not null references observation(observation_id) on delete cascade, -- a proposal is always tied to an observation
  taxon_id integer not null references taxon(taxon_id) on delete cascade, -- a proposal is always tied to a taxon (debateable)
  text string not null,
  pub_date integer
);