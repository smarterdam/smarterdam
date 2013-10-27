using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    /// <summary>
    /// Определяет временную метку, начиная с которой можно начинать тестирование модели
    /// </summary>
    public interface ITestStartDateProvider
    {
        DateTime GetTimestampOfTestStart(int measurementId);
    }
}
