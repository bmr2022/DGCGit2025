using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Helpers
{
    public class Base64Binder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            string base64Value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(base64Value))
            {
                bindingContext.Result = ModelBindingResult.Success(
                    bindingContext.ModelType == typeof(int) ? (object)0 : string.Empty
                );
                return Task.CompletedTask;
            }

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64Value));

                if (bindingContext.ModelType == typeof(int))
                {
                    if (int.TryParse(decoded, out var intValue))
                    {
                        bindingContext.Result = ModelBindingResult.Success(intValue);
                    }
                    else
                    {
                        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid integer value.");
                    }
                }
                else if (bindingContext.ModelType == typeof(string))
                {
                    bindingContext.Result = ModelBindingResult.Success(decoded);
                }
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Base64 value.");
            }

            return Task.CompletedTask;
        }
    }
}
