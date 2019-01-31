using System;
using Cocodrinks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cocodrinks.Utilities
{
    public class AccessHelper
    {
        internal static Int32 getAccessLevel(CocodrinksContext context,String userId)
        {
            if(userId != null){
                var user = context.Users.Find(Int32.Parse(userId));
                if(user != null){
                    return user.AccessLevel;
                }
            }
            return 99;
        }
    }
}