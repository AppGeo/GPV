//  Copyright 2012 Applied Geographics, Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading;

public class TimedCache<T>
{
  int _timeLimit;
  int _checkInterval;
  bool _removeOnRetrieve;

	Dictionary<String, TimedCacheEntry<T>> _cache;
  Thread _clearThread;

	public TimedCache(int timeLimit) : this(timeLimit, false) { }

	public TimedCache(int timeLimit, bool removeOnRetrieve) : this(timeLimit, 1, removeOnRetrieve) { }

	public TimedCache(int timeLimit, int checkInterval, bool removeOnRetrieve)
	{
		if (!typeof(T).IsClass)
		{
			throw new ArgumentException("Invalid type <" + typeof(T).Name + ">; the type stored in a TimedCache must be a class");
		}

    if (timeLimit < 1)
    {
      throw new ArgumentException("Time limit must be one or greater.");
    }

    if (checkInterval < 1)
    {
      throw new ArgumentException("Check interval must be one or greater.");
    }

		_timeLimit = timeLimit;
    _checkInterval = checkInterval * 1000;
		_removeOnRetrieve = removeOnRetrieve;

		_cache = new Dictionary<String, TimedCacheEntry<T>>();

    _clearThread = new Thread(new ThreadStart(ClearOutdatedEntries));
    _clearThread.Start();
	}

  ~TimedCache()
  {
    _clearThread.Abort();
  }

	private void ClearOutdatedEntries()
	{
    while (true)
    {
      DateTime minDate = DateTime.Now.AddSeconds(-_timeLimit);
      List<String> keys = new List<String>();

      lock (_cache)
      {
        foreach (string key in _cache.Keys)
        {
          TimedCacheEntry<T> timedCacheEntry = _cache[key];

          if (timedCacheEntry.LastAccessed <= minDate)
          {
            keys.Add(key);
          }
        }

        if (keys.Count > 0)
        {
          foreach (string key in keys)
          {
            _cache.Remove(key);
          }
        }
      }

      Thread.Sleep(_checkInterval);
    }
	}

	public T Retrieve(string key)
	{
		T item = default(T);

    lock (_cache)
		{
			if (_cache.ContainsKey(key))
			{
				TimedCacheEntry<T> timedCacheEntry = _cache[key];
				item = timedCacheEntry.Item;

				if (_removeOnRetrieve)
				{
					_cache.Remove(key);
				}
				else
				{
					timedCacheEntry.LastAccessed = DateTime.Now;
				}
			}
		}

		return item;
	}

	public string Store(T item)
	{
		DateTime d = DateTime.Now;
		Random r = new Random();

		string key = r.Next(999999).ToString("000000") + (d.Ticks - (new DateTime(d.Year, d.Month, d.Day)).Ticks).ToString("000000000000");
		return Store(key, item);
	}

	public string Store(string key, T item)
	{
    lock (_cache)
		{
			_cache[key] = new TimedCacheEntry<T>(item);
		}

		return key;
	}

	private struct TimedCacheEntry<ET>
	{
		public DateTime LastAccessed;
		public ET Item;

		public TimedCacheEntry(ET item)
		{
			LastAccessed = DateTime.Now;
			Item = item;
		}
	}
}
