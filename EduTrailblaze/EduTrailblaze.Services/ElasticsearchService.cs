using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Nest;

namespace EduTrailblaze.Services
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;

        public ElasticsearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task EnsureIndexExistsAsync(string indexName)
        {
            var existsResponse = await _client.Indices.ExistsAsync(indexName);
            if (!existsResponse.Exists)
            {
                await CreateIndexAsync(indexName);
            }
        }

        public async Task<bool> IsAvailableAsync()
        {
            var pingResponse = await _client.PingAsync();
            return pingResponse.IsValid; // Returns true if the ping is successful
        }

        public async Task DeleteIndexAsync(string indexName)
        {
            var deleteIndexResponse = await _client.Indices.DeleteAsync(indexName);
            if (!deleteIndexResponse.IsValid)
            {
                throw new Exception($"Error deleting index: {deleteIndexResponse.ServerError.Error.Reason}");
            }
            else
            {
                Console.WriteLine($"Index {indexName} deleted successfully.");
            }
        }

        public async Task CreateIndexAsync(string indexName)
        {
            // Check if the index exists
            var existsResponse = await _client.Indices.ExistsAsync(indexName);
            if (existsResponse.Exists)
            {
                Console.WriteLine($"Index {indexName} already exists.");
                return;
            }

            // Define index settings and mappings
            var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            //.Phonetic("my_phonetic_filter", ph => ph
                            //    .Encoder(PhoneticEncoder.DoubleMetaphone)
                            //    .Replace(false)
                            //)
                            .Synonym("synonym_filter_1", sf => sf
                                .Synonyms("IT, Information Technology")
                            )
                            .WordDelimiterGraph("word_delimiter_custom", wdg => wdg
                                .GenerateWordParts()
                                .GenerateNumberParts()
                                .CatenateWords()
                                .CatenateNumbers()
                                .CatenateAll()
                                .PreserveOriginal()
                            )
                            .NGram("ngram_filter", ng => ng
                                .MinGram(3)
                                .MaxGram(4)
                            )
                            .Stemmer("my_stemmer", st => st
                                .Language("english")
                            )
                            .Shingle("shingle_filter", sh => sh
                                .MinShingleSize(2)
                                .MaxShingleSize(3)
                            )
                        )
                        .Analyzers(an => an
                            .Custom("my_custom_analyzer", ca => ca
                                .Tokenizer("whitespace")
                                .Filters("lowercase", "synonym_filter_1",
                                         "word_delimiter_custom",
                                         //"my_phonetic_filter",
                                         "ngram_filter", "my_stemmer", "shingle_filter")
                            )
                        )
                    )
                )
                .Map(m => m
                    .Properties(ps => ps
                        .Text(t => t
                            .Name("Title")
                            .Analyzer("my_custom_analyzer")
                            .Fields(f => f
                                .Keyword(k => k
                                    .Name("raw")
                                )
                                .Text(ft => ft
                                    .Name("folded")
                                    .Analyzer("my_custom_analyzer")
                                )
                            )
                        )
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Error creating index: {createIndexResponse.ServerError.Error.Reason}");
            }

            Console.WriteLine($"Index {indexName} created successfully.");
        }

        public async Task ClearIndexAsync(string indexName)
        {
            var deleteIndexResponse = await _client.Indices.DeleteAsync(indexName);
            if (!deleteIndexResponse.IsValid)
            {
                throw new Exception($"Error deleting index: {deleteIndexResponse.ServerError.Error.Reason}");
            }
        }

        public async Task IndexCoursesAsync(List<CourseDTO> courses)
        {
            try
            {
                await ClearIndexAsync("courses");

                foreach (var course in courses)
                {
                    var indexResponse = await _client.IndexAsync(course, i => i
                        .Index("courses")
                        .Id(course.Id.ToString()) // Ensure unique ID
                    );

                    if (!indexResponse.IsValid)
                    {
                        Console.WriteLine($"Error indexing course {course.Id}: {indexResponse.ServerError.Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine($"Course {course.Id} indexed successfully");
                    }
                }

                // Force a refresh on the index
                var refreshResponse = await _client.Indices.RefreshAsync("courses");
                if (!refreshResponse.IsValid)
                {
                    Console.WriteLine($"Error refreshing index: {refreshResponse.ServerError.Error.Reason}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while indexing courses: " + ex.Message);
            }
        }


        public async Task<List<CourseDTO>> SearchCoursesByNameAsync(string name)
        {
            var response = await _client.SearchAsync<CourseDTO>(s => s
                .Index("courses")
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            sh => sh.Match(m => m
                                .Field(f => f.Title)
                                .Query(name)
                                .Fuzziness(Fuzziness.Auto)
                                .Operator(Operator.Or)
                            ),
                            sh => sh.Match(m => m
                                .Field(f => f.Title.Suffix("phonetic"))
                                .Query(name)
                                .Fuzziness(Fuzziness.Auto)
                            ),
                            sh => sh.MatchPhrase(mp => mp
                                .Field(f => f.Title)
                                .Query(name)
                                .Slop(1)
                            ),
                            sh => sh.Wildcard(wc => wc
                                .Field(f => f.Title)
                                .Value($"*{name}*")
                            )
                        )
                        .MinimumShouldMatch(1)
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Search failed: {response.ServerError.Error.Reason}");
            }
            if (!response.Documents.Any())
            {
                Console.WriteLine($"No courses found for title: {name}");
            }

            return response.Documents.ToList();
        }
    }
}
