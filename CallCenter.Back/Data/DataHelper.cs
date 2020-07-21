using CallCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallCenter.Back.Data
{
    public static class DataHelper
    {
        public static async Task AddTestData(DataBaseContext context)
        {
            context.Persons.RemoveRange(context.Persons);
            await context.SaveChangesAsync();

            var data = new List<Person>
            {
                new Person
                {
                    BirthDate = DateTime.Now.Date.AddYears(-27).AddMonths(5).AddDays(20), //27
                    FirstName = "Алена",
                    Patronymic = "Викторовна",
                    LastName = "Михайлюк",
                    Gender = Gender.Female,
                    PhoneNumber = "+380964521256",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-1),
                            OrderCost = 0,
                            CallReport = "Звонок прошел хорошо"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-7),
                            OrderCost = 0,
                            CallReport = "Опреатор вне зоны доступа"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-5),
                            OrderCost = 199.55,
                            CallReport = "Был сделан заказ"
                        }
                    }
                },
                new Person
                {
                    BirthDate = DateTime.Now.Date.AddYears(-19).AddMonths(3).AddDays(18), //19
                    FirstName = "Евгений",
                    Patronymic = "Павлович",
                    LastName = "Мостовой",
                    Gender = Gender.Male,
                    PhoneNumber = "+380502486363",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-15),
                            OrderCost = 55.12,
                            CallReport = "Часть заказа была с браком"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-10),
                            OrderCost = 0,
                            CallReport = "С 12 до 16 - неудобное время"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-8),
                            OrderCost = 230.00,
                            CallReport = "Повторный заказ 55"
                        }
                    }
                },
                    new Person()
                {
                    BirthDate = DateTime.Now.Date.AddYears(-13).AddMonths(11).AddDays(7), //13
                    FirstName = "Ольга",
                    Patronymic = "Владимировна",
                    LastName = "Кожемякина",
                    Gender = Gender.All,
                    PhoneNumber = "+380954562312",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-16),
                            OrderCost = 0,
                            CallReport = "Отмена повторного заказа"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-20),
                            OrderCost = 330.85,
                            CallReport = "Заказ из новой коллекции"
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-18),
                            OrderCost = 5,
                            CallReport = "Уточнение заказа и замена товара"
                        }
                    }
                },
                new Person()
                {
                    BirthDate = DateTime.Now.Date.AddYears(-17).AddMonths(6).AddDays(17), //17
                    FirstName = "Людмила",
                    Patronymic = "Семеновна",
                    LastName = "Довгополая",
                    Gender = Gender.All,
                    PhoneNumber = "+380975362141",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-16),
                            OrderCost = 234.32,
                            CallReport = "Fusce varius placerat finibus. Etiam finibus, nisl iaculis facilisis scelerisque, ipsum mi ultricies ipsum, vel commodo leo nisi quis odio. Donec non ante vitae felis mollis tincidunt. Pellentesque fermentum sollicitudin ultrices. In hac habitasse platea dictumst. Vestibulum tristique in odio nec dignissim. Nulla ultricies libero id risus malesuada tempus."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-20),
                            OrderCost = 740,
                            CallReport = "Praesent eget metus porttitor dui volutpat vulputate. Etiam in sapien in felis rhoncus malesuada in vel risus. Sed vestibulum fermentum lectus sed pretium. Aliquam imperdiet luctus massa sed tincidunt. Nam purus magna, molestie in nibh et, vulputate accumsan massa. Quisque ultricies elit quis nulla sollicitudin, a sodales orci interdum. Phasellus non urna aliquam, rhoncus nibh quis, egestas ante."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-18),
                            OrderCost = 580,
                            CallReport = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin interdum faucibus turpis, vel ultrices nisi laoreet sed. Sed a sapien eu felis consectetur pretium. Suspendisse ac enim quis lorem egestas mollis ac in felis. Pellentesque et mi gravida, sollicitudin ante efficitur, dapibus nunc. Donec sed orci auctor nibh malesuada pellentesque. Sed vulputate maximus vestibulum. Curabitur viverra magna a mi tristique, quis ultricies augue semper.
                                           Donec euismod nunc nisi, ut vulputate velit commodo blandit. Suspendisse potenti. In pretium bibendum nulla, quis lacinia risus scelerisque eu. Duis nec quam ac tortor gravida efficitur nec vitae turpis. In efficitur ligula ut leo posuere, eget tempus erat congue. Pellentesque vehicula nibh vel nisi mattis congue. Mauris sed ante aliquam, tempor tortor et, pharetra ante. Vivamus gravida sit amet quam sit amet vestibulum. Sed sagittis ultrices diam, at interdum magna lobortis vel. Aliquam semper dolor sagittis, fringilla nunc at, dictum purus. Duis eget blandit metus. Nunc vel aliquam ante, consequat hendrerit urna. Phasellus maximus tincidunt gravida. Pellentesque ut tempus massa."
                        }
                    }
                },
                new Person()
                {
                    BirthDate = DateTime.Now.Date.AddYears(-24).AddMonths(0).AddDays(9), 
                    FirstName = "Семен",
                    Patronymic = "Сергеевич",
                    LastName = "Дуров",
                    Gender = Gender.Male,
                    PhoneNumber = "+38097354678",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-10),
                            OrderCost = 333,
                            CallReport = "Fusce varius placerat finibus. Etiam finibus, nisl iaculis facilisis scelerisque, ipsum mi ultricies ipsum, vel commodo leo nisi quis odio. Donec non ante vitae felis mollis tincidunt. Pellentesque fermentum sollicitudin ultrices. In hac habitasse platea dictumst. Vestibulum tristique in odio nec dignissim. Nulla ultricies libero id risus malesuada tempus."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-2),
                            OrderCost = 120.15,
                            CallReport = "Praesent eget metus porttitor dui volutpat vulputate. Etiam in sapien in felis rhoncus malesuada in vel risus. Sed vestibulum fermentum lectus sed pretium. Aliquam imperdiet luctus massa sed tincidunt. Nam purus magna, molestie in nibh et, vulputate accumsan massa. Quisque ultricies elit quis nulla sollicitudin, a sodales orci interdum. Phasellus non urna aliquam, rhoncus nibh quis, egestas ante."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-60),
                            OrderCost = 30,
                            CallReport = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin interdum faucibus turpis, vel ultrices nisi laoreet sed. Sed a sapien eu felis consectetur pretium. Suspendisse ac enim quis lorem egestas mollis ac in felis. Pellentesque et mi gravida, sollicitudin ante efficitur, dapibus nunc. Donec sed orci auctor nibh malesuada pellentesque. Sed vulputate maximus vestibulum. Curabitur viverra magna a mi tristique, quis ultricies augue semper.
                                           Donec euismod nunc nisi, ut vulputate velit commodo blandit. Suspendisse potenti. In pretium bibendum nulla, quis lacinia risus scelerisque eu. Duis nec quam ac tortor gravida efficitur nec vitae turpis. In efficitur ligula ut leo posuere, eget tempus erat congue. Pellentesque vehicula nibh vel nisi mattis congue. Mauris sed ante aliquam, tempor tortor et, pharetra ante. Vivamus gravida sit amet quam sit amet vestibulum. Sed sagittis ultrices diam, at interdum magna lobortis vel. Aliquam semper dolor sagittis, fringilla nunc at, dictum purus. Duis eget blandit metus. Nunc vel aliquam ante, consequat hendrerit urna. Phasellus maximus tincidunt gravida. Pellentesque ut tempus massa."
                        }
                    }
                },
                new Person()
                {
                    BirthDate = DateTime.Now.Date.AddYears(-15).AddMonths(3).AddDays(17),
                    FirstName = "Милана",
                    Patronymic = "Викторовна",
                    LastName = "Жовнир",
                    Gender = Gender.Female,
                    PhoneNumber = "+38098340999",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-1),
                            OrderCost = 256,
                            CallReport = "Fusce varius placerat finibus. Etiam finibus, nisl iaculis facilisis scelerisque, ipsum mi ultricies ipsum, vel commodo leo nisi quis odio. Donec non ante vitae felis mollis tincidunt. Pellentesque fermentum sollicitudin ultrices. In hac habitasse platea dictumst. Vestibulum tristique in odio nec dignissim. Nulla ultricies libero id risus malesuada tempus."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(0),
                            OrderCost = 45,
                            CallReport = "Praesent eget metus porttitor dui volutpat vulputate. Etiam in sapien in felis rhoncus malesuada in vel risus. Sed vestibulum fermentum lectus sed pretium. Aliquam imperdiet luctus massa sed tincidunt. Nam purus magna, molestie in nibh et, vulputate accumsan massa. Quisque ultricies elit quis nulla sollicitudin, a sodales orci interdum. Phasellus non urna aliquam, rhoncus nibh quis, egestas ante."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-12),
                            OrderCost = 0,
                            CallReport = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin interdum faucibus turpis, vel ultrices nisi laoreet sed. Sed a sapien eu felis consectetur pretium. Suspendisse ac enim quis lorem egestas mollis ac in felis. Pellentesque et mi gravida, sollicitudin ante efficitur, dapibus nunc. Donec sed orci auctor nibh malesuada pellentesque. Sed vulputate maximus vestibulum. Curabitur viverra magna a mi tristique, quis ultricies augue semper.
                                           Donec euismod nunc nisi, ut vulputate velit commodo blandit. Suspendisse potenti. In pretium bibendum nulla, quis lacinia risus scelerisque eu. Duis nec quam ac tortor gravida efficitur nec vitae turpis. In efficitur ligula ut leo posuere, eget tempus erat congue. Pellentesque vehicula nibh vel nisi mattis congue. Mauris sed ante aliquam, tempor tortor et, pharetra ante. Vivamus gravida sit amet quam sit amet vestibulum. Sed sagittis ultrices diam, at interdum magna lobortis vel. Aliquam semper dolor sagittis, fringilla nunc at, dictum purus. Duis eget blandit metus. Nunc vel aliquam ante, consequat hendrerit urna. Phasellus maximus tincidunt gravida. Pellentesque ut tempus massa."
                        }
                    }
                },
                new Person()
                {
                    BirthDate = DateTime.Now.Date.AddYears(-34).AddMonths(5).AddDays(9),
                    FirstName = "Ася",
                    Patronymic = "Романовна",
                    LastName = "Дорожнюк",
                    Gender = Gender.Female,
                    PhoneNumber = "+3809999890",
                    Calls = new List<Call>
                    {
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(0),
                            OrderCost = 0,
                            CallReport = "Fusce varius placerat finibus. Etiam finibus, nisl iaculis facilisis scelerisque, ipsum mi ultricies ipsum, vel commodo leo nisi quis odio. Donec non ante vitae felis mollis tincidunt. Pellentesque fermentum sollicitudin ultrices. In hac habitasse platea dictumst. Vestibulum tristique in odio nec dignissim. Nulla ultricies libero id risus malesuada tempus."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-8),
                            OrderCost = 1560,
                            CallReport = "Praesent eget metus porttitor dui volutpat vulputate. Etiam in sapien in felis rhoncus malesuada in vel risus. Sed vestibulum fermentum lectus sed pretium. Aliquam imperdiet luctus massa sed tincidunt. Nam purus magna, molestie in nibh et, vulputate accumsan massa. Quisque ultricies elit quis nulla sollicitudin, a sodales orci interdum. Phasellus non urna aliquam, rhoncus nibh quis, egestas ante."
                        },
                        new Call
                        {
                            CallDate = DateTime.Now.AddDays(-45),
                            OrderCost = 870,
                            CallReport = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin interdum faucibus turpis, vel ultrices nisi laoreet sed. Sed a sapien eu felis consectetur pretium. Suspendisse ac enim quis lorem egestas mollis ac in felis. Pellentesque et mi gravida, sollicitudin ante efficitur, dapibus nunc. Donec sed orci auctor nibh malesuada pellentesque. Sed vulputate maximus vestibulum. Curabitur viverra magna a mi tristique, quis ultricies augue semper.
                                           Donec euismod nunc nisi, ut vulputate velit commodo blandit. Suspendisse potenti. In pretium bibendum nulla, quis lacinia risus scelerisque eu. Duis nec quam ac tortor gravida efficitur nec vitae turpis. In efficitur ligula ut leo posuere, eget tempus erat congue. Pellentesque vehicula nibh vel nisi mattis congue. Mauris sed ante aliquam, tempor tortor et, pharetra ante. Vivamus gravida sit amet quam sit amet vestibulum. Sed sagittis ultrices diam, at interdum magna lobortis vel. Aliquam semper dolor sagittis, fringilla nunc at, dictum purus. Duis eget blandit metus. Nunc vel aliquam ante, consequat hendrerit urna. Phasellus maximus tincidunt gravida. Pellentesque ut tempus massa."
                        }
                    }
                }

            };

            await context.Persons.AddRangeAsync(data);
            await context.SaveChangesAsync();
        }
    }
}
