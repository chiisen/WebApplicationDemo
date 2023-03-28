/*
-- Query: SELECT * FROM dev.WeatherForecast
LIMIT 0, 1000

-- Date: 2023-03-28 10:30
*/

CREATE TABLE WeatherForecast (
  Summaries varchar(16) NOT NULL,
  CreateDate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
);



INSERT INTO WeatherForecast (Summaries) VALUES ('Balmy');
INSERT INTO WeatherForecast (Summaries) VALUES ('Bracing');
INSERT INTO WeatherForecast (Summaries) VALUES ('Chilly');
INSERT INTO WeatherForecast (Summaries) VALUES ('Cool');
INSERT INTO WeatherForecast (Summaries) VALUES ('Freezing');
INSERT INTO WeatherForecast (Summaries) VALUES ('Hot');
INSERT INTO WeatherForecast (Summaries) VALUES ('Mild');
INSERT INTO WeatherForecast (Summaries) VALUES ('Scorching');
INSERT INTO WeatherForecast (Summaries) VALUES ('Sweltering');
INSERT INTO WeatherForecast (Summaries) VALUES ('Warm');
