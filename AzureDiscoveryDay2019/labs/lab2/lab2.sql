create table dbo.payment_type
(
	payment_type int null,
	abbreviation varchar(50) null,
	description varchar(50) null
);
go

create table dbo.rate_code
(
	rate_code_id int null,
	description varchar(50) null
);
go

create table dbo.taxi_zone
(
	location_id varchar(50) null,
	borough varchar(50) null,
	zone varchar(50) null,
	service_zone varchar(50) null
);
go

create table dbo.trip_month
(
	trip_month varchar(50) null,
	month_name_short varchar(50) null,
	month_name_full varchar(50) null
);
go

create table dbo.trip_type
(
	trip_type int null,
	description varchar(50) null
);
go

create table dbo.vendor
(
	vendor_id int null,
	abbreviation varchar(50) null,
	description varchar(50) null
);
go


create table dbo.trips_all
(
	trip_type int null,
	trip_year int null,
	trip_month varchar(100) null,
	taxi_type varchar(100) null,
	vendor_id int null,
	pickup_datetime datetime null,
	dropoff_datetime datetime null,
	passenger_count int null,
	trip_distance float null,
	rate_code_id int null,
	store_and_fwd_flag varchar(100) null,
	pickup_location_id int null,
	dropoff_location_id int null,
	pickup_longitude varchar(100) null,
	pickup_latitude varchar(100) null,
	dropoff_longitude varchar(100) null,
	dropoff_latitude varchar(100) null,
	payment_type int null,
	fare_amount float null,
	extra float null,
	mta_tax float null,
	tip_amount float null,
	tolls_amount float null,
	improvement_surcharge float null,
	ehail_fee float null,
	total_amount float null
);
go

create index ix_trip_year_trip_month
on dbo.trips_all 
(
	trip_year, trip_month
);
go

create table dbo.trips_new
(
	trip_type int null,
	trip_year int null,
	trip_month varchar(100) null,
	taxi_type varchar(100) null,
	vendor_id int null,
	pickup_datetime datetime null,
	dropoff_datetime datetime null,
	passenger_count int null,
	trip_distance float null,
	rate_code_id int null,
	store_and_fwd_flag varchar(100) null,
	pickup_location_id int null,
	dropoff_location_id int null,
	pickup_longitude varchar(100) null,
	pickup_latitude varchar(100) null,
	dropoff_longitude varchar(100) null,
	dropoff_latitude varchar(100) null,
	payment_type int null,
	fare_amount float null,
	extra float null,
	mta_tax float null,
	tip_amount float null,
	tolls_amount float null,
	improvement_surcharge float null,
	ehail_fee float null,
	total_amount float null,
	textanalytics_customer_sentiment_score float null,
	customer_comments nvarchar(max) null
);
go

create index ix_trip_year_trip_month
on dbo.trips_new 
(
	trip_year, trip_month
);
go

create view dbo.trips as
select
	trip_type,
	trip_year,
	trip_month,
	taxi_type,
	vendor_id,
	pickup_datetime,
	dropoff_datetime,
	passenger_count,
	trip_distance,
	rate_code_id,
	store_and_fwd_flag,
	pickup_location_id,
	dropoff_location_id,
	pickup_longitude,
	pickup_latitude,
	dropoff_longitude,
	dropoff_latitude,
	payment_type,
	fare_amount,
	extra,
	mta_tax,
	tip_amount,
	tolls_amount,
	improvement_surcharge,
	ehail_fee,
	total_amount,
	textanalytics_customer_sentiment_score = null,
	customer_comments = null
from
	dbo.trips_all
union all
select
	trip_type,
	trip_year,
	trip_month,
	taxi_type,
	vendor_id,
	pickup_datetime,
	dropoff_datetime,
	passenger_count,
	trip_distance,
	rate_code_id,
	store_and_fwd_flag,
	pickup_location_id,
	dropoff_location_id,
	pickup_longitude,
	pickup_latitude,
	dropoff_longitude,
	dropoff_latitude,
	payment_type,
	fare_amount,
	extra,
	mta_tax,
	tip_amount,
	tolls_amount,
	improvement_surcharge,
	ehail_fee,
	total_amount,
	textanalytics_customer_sentiment_score,
	customer_comments
from
	dbo.trips_new
;
go

create table dbo.trips_all_exp
(
	trip_type int null,
	trip_year int null,
	trip_month varchar(100) null,
	taxi_type varchar(100) null,
	vendor_id int null,
	pickup_datetime datetime null,
	dropoff_datetime datetime null,
	passenger_count int null,
	trip_distance float null,
	rate_code_id int null,
	store_and_fwd_flag varchar(100) null,
	pickup_location_id int null,
	dropoff_location_id int null,
	pickup_longitude varchar(100) null,
	pickup_latitude varchar(100) null,
	dropoff_longitude varchar(100) null,
	dropoff_latitude varchar(100) null,
	payment_type int null,
	fare_amount float null,
	extra float null,
	mta_tax float null,
	tip_amount float null,
	tolls_amount float null,
	improvement_surcharge float null,
	ehail_fee float null,
	total_amount float null,
	textanalytics_customer_sentiment_score float null,
	customer_comments nvarchar(max) null,
    payment_type_description varchar(50) null,
    rate_code_description varchar(50) null,
    pickup_borough varchar(50) null,
    pickup_zone varchar(50) null,
    pickup_service_zone varchar(50) null,
    dropoff_borough varchar(50) null,
    dropoff_zone varchar(50) null,
    dropoff_service_zone varchar(50) null,
    month_name_short varchar(50) null,
    month_name_full varchar(50) null,
    trip_type_description varchar(50) null,
    vendor_abbreviation varchar(50) null,
    vendor_description varchar(50) null
);
go

create index ix_trip_year_trip_month
on dbo.trips_all_exp
(
	trip_year, trip_month
);
go
