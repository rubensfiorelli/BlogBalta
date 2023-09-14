﻿namespace Blog.DTOs
{
    public class ListPostRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
