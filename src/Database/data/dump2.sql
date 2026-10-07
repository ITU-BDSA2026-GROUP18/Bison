INSERT INTO taxon(taxon_id, parent_taxon_id, dwc_taxon_id, vernacular_name) VALUES(1, NULL, 'MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea', 'Årefodede');
INSERT INTO taxon(taxon_id, parent_taxon_id, dwc_taxon_id, vernacular_name) VALUES(2, 1, 'MSTSNM:Arter:495067e4-f785-ea11-aa77-501ac539d1ea', 'Hejrer');
INSERT INTO taxon(taxon_id, parent_taxon_id, dwc_taxon_id, vernacular_name) VALUES(3, 1, 'MSTSNM:Arter:a15367e4-f785-ea11-aa77-501ac539d1ea', 'Fregatfugle');

INSERT INTO user(user_id, username, email, pw_hash) VALUES(1, 'Roger Histand', 'Roger+Histand@hotmail.com', '');
INSERT INTO user(user_id, username, email, pw_hash) VALUES(2, 'Luanna Muro', 'Luanna-Muro@ku.dk', '');

INSERT INTO post(post_id, author_id, text, pub_date) VALUES(1, 1, 'Grey heron.', 1690873920);
INSERT INTO post(post_id, author_id, text, pub_date) VALUES(2, 2, 'Flock of about 40 cormorants.', 1690878600);
INSERT INTO post(post_id, author_id, text, pub_date) VALUES(3, 1, 'Large white heron in a wet meadow.', 1690958700);
INSERT INTO post(post_id, author_id, text, pub_date) VALUES(4, 2, 'Gannets diving offshore.', 1690974300);
INSERT INTO post(post_id, author_id, text, pub_date) VALUES(5, 1, 'Heard a booming call from the reed bed at dusk.', 1691097000);

INSERT INTO observation(post_id, taxon_id) VALUES(1, 1);
INSERT INTO observation(post_id, taxon_id) VALUES(2, 1);
INSERT INTO observation(post_id, taxon_id) VALUES(3, 2);
INSERT INTO observation(post_id, taxon_id) VALUES(4, 2);
INSERT INTO observation(post_id, taxon_id) VALUES(5, 3);
