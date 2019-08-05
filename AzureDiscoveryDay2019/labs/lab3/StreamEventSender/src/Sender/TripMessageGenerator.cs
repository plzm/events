using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using pelazem.rndgen;
using pelazem.util;

namespace Sender
{
	public class TripMessageGenerator
	{
		#region Variables

		private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include, Formatting = Formatting.None };

		#endregion

		#region Generation

		private int[] _trip_types = { 1, 2 };
		private string _trip_year = DateTime.Now.Year.ToString();
		private string _trip_month = string.Format("{0:MM}", DateTime.Now);
		private string[] _taxi_types = { "Yellow", "Green" };
		private int[] _vendor_ids = { 1, 2, 3 };

		private Category<int>[] _passenger_counts =
		{
			new Category<int>(){Value = 1, RelativeWeight = 18},
			new Category<int>(){Value = 2, RelativeWeight = 15},
			new Category<int>(){Value = 3, RelativeWeight = 13},
			new Category<int>(){Value = 4, RelativeWeight = 9},
			new Category<int>(){Value = 5, RelativeWeight = 5},
			new Category<int>(){Value = 6, RelativeWeight = 2},
			new Category<int>(){Value = 7, RelativeWeight = 0.2},
			new Category<int>(){Value = 8, RelativeWeight = 0.1},
			new Category<int>(){Value = 9, RelativeWeight = 0.05},
			new Category<int>(){Value = 10, RelativeWeight = 0.01}
		};

		private Category<int>[] _rate_codes =
		{
			new Category<int>(){Value = 1, RelativeWeight = 20},
			new Category<int>(){Value = 2, RelativeWeight = 1},
			new Category<int>(){Value = 3, RelativeWeight = 1},
			new Category<int>(){Value = 4, RelativeWeight = 0.2},
			new Category<int>(){Value = 5, RelativeWeight = 0.2},
			new Category<int>(){Value = 6, RelativeWeight = 15}
		};

		private Category<string>[] _store_and_fwd_flags =
		{
			new Category<string>(){Value = "Y", RelativeWeight = 1},
			new Category<string>(){Value = "N", RelativeWeight = 1},
			new Category<string>(){Value = string.Empty, RelativeWeight = 50}
		};

		private Category<int>[] _payment_types =
		{
			new Category<int>(){Value = 1, RelativeWeight = 1000},
			new Category<int>(){Value = 2, RelativeWeight = 900},
			new Category<int>(){Value = 3, RelativeWeight = 20},
			new Category<int>(){Value = 4, RelativeWeight = 2},
			new Category<int>(){Value = 5, RelativeWeight = 0.1},
			new Category<int>(){Value = 6, RelativeWeight = 10},
			new Category<int>(){Value = 7, RelativeWeight = 40},
			new Category<int>(){Value = 8, RelativeWeight = 40},
			new Category<int>(){Value = 9, RelativeWeight = 30},
			new Category<int>(){Value = 10, RelativeWeight = 30},
			new Category<int>(){Value = 11, RelativeWeight = 30},
			new Category<int>(){Value = 12, RelativeWeight = 25},
			new Category<int>(){Value = 13, RelativeWeight = 25},
			new Category<int>(){Value = 14, RelativeWeight = 25},
			new Category<int>(){Value = 15, RelativeWeight = 25},
			new Category<int>(){Value = 16, RelativeWeight = 25},
			new Category<int>(){Value = 17, RelativeWeight = 10},
			new Category<int>(){Value = 18, RelativeWeight = 10},
			new Category<int>(){Value = 19, RelativeWeight = 10},
			new Category<int>(){Value = 20, RelativeWeight = 1},
			new Category<int>(){Value = 21, RelativeWeight = 2},
			new Category<int>(){Value = 22, RelativeWeight = 2},
			new Category<int>(){Value = 23, RelativeWeight = 2},
			new Category<int>(){Value = 24, RelativeWeight = 1},
		};

		private Category<double>[] _extras =
		{
			new Category<double>(){Value = 0, RelativeWeight = 20},
			new Category<double>(){Value = 1.50, RelativeWeight = 1},
			new Category<double>(){Value = 3.25, RelativeWeight = 1},
			new Category<double>(){Value = 4.80, RelativeWeight = 0.2}
		};

		private Category<double>[] _tip_proportions =
		{
			new Category<double>(){Value = 0, RelativeWeight = 2},
			new Category<double>(){Value = 0.15, RelativeWeight = 20},
			new Category<double>(){Value = 0.1, RelativeWeight = 5},
			new Category<double>(){Value = 0.2, RelativeWeight = 7},
			new Category<double>(){Value = 0.25, RelativeWeight = 3}
		};

		private Category<double>[] _toll_amounts =
		{
			new Category<double>(){Value = 0, RelativeWeight = 25},
			new Category<double>(){Value = 5.00, RelativeWeight = 1 },
			new Category<double>(){Value = 8.00, RelativeWeight = 1},
			new Category<double>(){Value = 17.00, RelativeWeight = 1}
		};

		private Category<double>[] _improvement_surcharges =
		{
			new Category<double>(){Value = 0, RelativeWeight = 50},
			new Category<double>(){Value = 1.50, RelativeWeight = 1 },
			new Category<double>(){Value = 3.00, RelativeWeight = 1}
		};

		private Category<double>[] _ehail_fees =
		{
			new Category<double>(){Value = 0, RelativeWeight = 50},
			new Category<double>(){Value = 2.25, RelativeWeight = 1 },
			new Category<double>(){Value = 3.00, RelativeWeight = 1}
		};

		private string[] _customer_comments =
		{
			"Trip was great! Driver was polite and we got there faster than expected.",
			"Good drive. Got there on time.",
			"Driver got upset with heavy traffic and yelled a lot. It was very unpleasant.",
			"Got stuck in heavy traffic, I was late. Driver should have checked GPS to find a faster route!",
			"Taxi was too small for my bags. Next time send bigger taxi or I will use a ride share instead.",
			"Car was an ugly color, was embarrassing and dirty. Try a car wash!",
			"Fast, good",
			"Car was dirty and smelled bad. Get it detailed",
			"Driver drove too fast, ran red lights, did not feel safe",
			"Hailed taxi, pulled over, helped me load, nice!",
			"The app is bad but taxi was very good, driver polite and funny, got me and my friends laughing",
			"Not enough taxis at night, took too long to get to us",
			"Driver was nice",
			"Car was brand new, very pleasant drive",
			"YOu need a better app!",
			"Car was very hot, black interior on summer day, no AC!",
			"Seats were wet from rain, no towel, cold, not happy!",
			"This is why I don't need a car, great taxis, happy drivers, low costs! No, I don't work for the taxi company :)",
			"Why do I have to fill out yet another survey? Leave me alone",
			"Just do your job and it will be fine",
			"The driver went out of our way so I could pick up dry cleaning, dog food, and a blankie - wow send him every time!",
			"Taxi was late, driver was rude, weather was bad, and I have a zit! Ready for tomorrow",
			"Driver played loud fast music. This woke me up too much!",
			"Too many taxis - get rid of some so the rest can go faster on less crowded streets",
			"Why no coffee machines in cars? Make this happen!",
			"Taxi got me home after night out bar hopping, yay!",
			"My driver tried to convert me - WTH? Just drive",
			"Is Penn Station really THAT hard to find? Incompetent - train your drivers",
			"The driver rocked it! Is that a red cape I see peeking out of his collar?",
			"In London our cabs are bigger, better, and not so wildly coloured. Please do try to get it right"
		};


		#endregion


		public string GetMessage()
		{
			TripMessage message = new TripMessage();

			message.trip_type = RandomGenerator.Categorical.GetCategorical<int>(_trip_types);
			message.trip_year = _trip_year;
			message.trip_month = _trip_month;
			message.taxi_type = RandomGenerator.Categorical.GetCategorical<string>(_taxi_types);
			message.vendor_id = RandomGenerator.Categorical.GetCategorical<int>(_vendor_ids);
			message.pickup_datetime = DateTime.Now.AddMinutes(RandomGenerator.Numeric.GetUniform(-60, -5));
			message.dropoff_datetime = DateTime.Now.AddSeconds(RandomGenerator.Numeric.GetUniform(-240, -5));
			message.passenger_count = RandomGenerator.Categorical.GetCategorical<int>(_passenger_counts);
			message.trip_distance = RandomGenerator.Numeric.GetNormal(2.5, 2);
			message.rate_code_id = RandomGenerator.Categorical.GetCategorical<int>(_rate_codes);
			message.store_and_fwd_flag = RandomGenerator.Categorical.GetCategorical<string>(_store_and_fwd_flags);
			message.pickup_location_id = Converter.GetInt32(RandomGenerator.Numeric.GetUniform(0, 265));
			message.dropoff_location_id = Converter.GetInt32(RandomGenerator.Numeric.GetUniform(0, 265));
			message.pickup_longitude = string.Format("{0:n6}", RandomGenerator.Numeric.GetUniform(-72.000000, -74.900000));
			message.pickup_latitude = string.Format("{0:n6}", RandomGenerator.Numeric.GetUniform(39.000000, 41.000000));
			message.dropoff_longitude = string.Format("{0:n6}", RandomGenerator.Numeric.GetUniform(-72.000000, -74.900000));
			message.dropoff_latitude = string.Format("{0:n6}", RandomGenerator.Numeric.GetUniform(39.000000, 41.000000));
			message.payment_type = RandomGenerator.Categorical.GetCategorical<int>(_payment_types);
			message.fare_amount = Math.Truncate(100 * RandomGenerator.Numeric.GetNormal(18, 6.75)) / 100;
			message.extra = RandomGenerator.Categorical.GetCategorical<double>(_extras);
			message.mta_tax = Math.Truncate(100 * 0.05 * message.fare_amount) / 100;
			message.tip_amount = Math.Truncate(100 * RandomGenerator.Categorical.GetCategorical<double>(_tip_proportions) * message.fare_amount) / 100;
			message.tolls_amount = RandomGenerator.Categorical.GetCategorical<double>(_toll_amounts);
			message.improvement_surcharge = RandomGenerator.Categorical.GetCategorical<double>(_improvement_surcharges);
			message.ehail_fee = RandomGenerator.Categorical.GetCategorical<double>(_ehail_fees);

			message.customer_comments = RandomGenerator.Categorical.GetCategorical<string>(_customer_comments);

			return JsonConvert.SerializeObject(message, _jsonSerializerSettings);
		}

	}
}
