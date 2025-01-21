using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySurveys.Shared.Models.Questions;

public class SurveyAnswer
{
    public int Id;
    public string[] Answers { get; set; }
}
