﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Food.Data;
using YellowCarrot.Food.Models;

namespace YellowCarrot.Food.Services
{
    internal class TagsRepo
    {
        private FoodDbContext context;

        public TagsRepo(FoodDbContext context)
        {
            this.context = context;
        }

        /* Takes a list of primitive Tags (i.e ones that arent linked to
         * the databases) and finds tracked versions of them. */
        public async Task<List<Tag>> GetMatchingTagsByName(List<string> userInputTagNames)
        {
            Func<string, string> capitalizeFirstLetter = x => $"{char.ToUpper(x[0])}{x.Substring(1)}";
            
            // "Converts" to correct grammar tag names for use in finding in database
            List<string> inputTagNamesListCapitalized = new();
            foreach (string tagName in userInputTagNames)
            {
                inputTagNamesListCapitalized.Add(capitalizeFirstLetter(tagName));
            }

            // Finds tags in the database with same name as those in the user written tags
            List<Tag> existingTagsInDatabase = await context.Tags.Where(t => inputTagNamesListCapitalized.Contains(t.TagName)).ToListAsync();

            return existingTagsInDatabase;
        }
    }
}
