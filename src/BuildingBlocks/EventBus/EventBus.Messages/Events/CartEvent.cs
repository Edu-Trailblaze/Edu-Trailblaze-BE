using EventBus.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public record CartEvent() : IntegrationBaseEvent,ICartEvent
    {
        public int CourseId { get ; set ; }

    }
    public class GetCourseRequest
    {
        public int CourseId { get; set; }
    }
    public class CartCourseInformation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int Duration { get; set; }
        public string DifficultyLevel { get; set; }
        public int TotalLectures { get; set; }
    }
}
