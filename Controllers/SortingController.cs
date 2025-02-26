using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortingController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSortingData(int iterations = 10, int maxDatasetSize = 5000)
        {
            var selectionSortTimes = new List<double>();
            var mergeSortTimes = new List<double>();
            var datasetSizes = new List<int>();

            for (int size = 0; size <= maxDatasetSize; size += 1000)
            {
                var people = GenerateRandomPeople(size);

                // Measure SelectionSort execution time
                var stopwatch = Stopwatch.StartNew();
                SelectionSort(people.ToList());
                stopwatch.Stop();
                selectionSortTimes.Add(stopwatch.ElapsedMilliseconds);

                // Measure MergeSort execution time
                stopwatch.Restart();
                MergeSort(people.ToList());
                stopwatch.Stop();
                mergeSortTimes.Add(stopwatch.ElapsedMilliseconds);

                datasetSizes.Add(size);
            }

            return Ok(new { SelectionSortTimes = selectionSortTimes, MergeSortTimes = mergeSortTimes, DatasetSizes = datasetSizes });
        }

        public static List<Person> GenerateRandomPeople(int count)
        {
            var random = new Random();
            return Enumerable.Range(0, count)
                .Select(i => new Person($"Person{i}", random.Next(10, 100)))
                .ToList();
        }

        // Selection Sort Implementation
        public static List<Person> SelectionSort(List<Person> people)
        {
            int n = people.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (people[j].CompareTo(people[minIndex]) < 0)
                    {
                        minIndex = j;
                    }
                }
                (people[i], people[minIndex]) = (people[minIndex], people[i]);
            }
            return people;
        }

        // Merge Sort Implementation
        public static List<Person> MergeSort(List<Person> people)
        {
            if (people.Count <= 1)
                return people;

            int middle = people.Count / 2;
            var left = people.Take(middle).ToList();
            var right = people.Skip(middle).ToList();

            return Merge(MergeSort(left), MergeSort(right));
        }

        private static List<Person> Merge(List<Person> left, List<Person> right)
        {
            var result = new List<Person>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (left[i].CompareTo(right[j]) <= 0)
                {
                    result.Add(left[i++]);
                }
                else
                {
                    result.Add(right[j++]);
                }
            }

            result.AddRange(left.Skip(i));
            result.AddRange(right.Skip(j));

            return result;
        }

        public class Person : IComparable<Person>
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public int CompareTo(Person other)
            {
                if (other == null) return 1;
                return Age.CompareTo(other.Age);
            }
        }

        public int CompareTo(str1, str2)
        {
            
        }
    }
}
