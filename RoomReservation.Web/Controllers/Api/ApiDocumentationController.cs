using Microsoft.AspNetCore.Mvc;
using RoomReservation.Common.DTOs;
using System.Reflection;

namespace RoomReservation.Web.Controllers.Api
{
	[ApiController]
	[Route("api/docs")]
	public class ApiDocumentationController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetApiDocumentation()
		{
			var controllerTypes = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type =>
					typeof(ControllerBase).IsAssignableFrom(type)
					&& type.Name.EndsWith("Controller")
					&& type.Namespace != null
					&& type.Namespace.Contains(".Api"))
				.ToList();

			var endpoints = new List<ApiEndpointDescription>();

			foreach (var controllerType in controllerTypes)
			{
				string controllerRoute = GetControllerRoute(controllerType);

				var methods = controllerType
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.Where(method => method.GetCustomAttributes().Any(IsHttpMethodAttribute));

				foreach (var method in methods)
				{
					string httpMethod = GetHttpMethod(method);
					string actionRoute = GetActionRoute(method);
					string fullRoute = CombineRoutes(controllerRoute, actionRoute);

					var parameters = method.GetParameters()
						.Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}")
						.ToList();

					endpoints.Add(new ApiEndpointDescription
					{
						Controller = controllerType.Name,
						Action = method.Name,
						HttpMethod = httpMethod,
						Route = fullRoute,
						ReturnType = GetReadableTypeName(method.ReturnType),
						Parameters = parameters
					});
				}
			}

			return Ok(endpoints.OrderBy(e => e.Route).ThenBy(e => e.HttpMethod));
		}

		private static string GetReadableTypeName(Type type)
		{
			if (!type.IsGenericType)
			{
				return type.Name;
			}

			string typeName = type.Name.Split('`')[0];

			var genericArguments = type.GetGenericArguments()
				.Select(GetReadableTypeName);

			return $"{typeName}<{string.Join(", ", genericArguments)}>";
		}

		private static bool IsHttpMethodAttribute(Attribute attribute)
		{
			return attribute is HttpGetAttribute
				|| attribute is HttpPostAttribute
				|| attribute is HttpPutAttribute
				|| attribute is HttpDeleteAttribute
				|| attribute is HttpPatchAttribute;
		}

		private static string GetControllerRoute(Type controllerType)
		{
			var routeAttribute = controllerType.GetCustomAttribute<RouteAttribute>();

			if (routeAttribute == null)
			{
				return string.Empty;
			}

			return routeAttribute.Template ?? string.Empty;
		}

		private static string GetActionRoute(MethodInfo method)
		{
			var httpAttribute = method.GetCustomAttributes()
				.FirstOrDefault(IsHttpMethodAttribute);

			return httpAttribute switch
			{
				HttpGetAttribute get => get.Template ?? string.Empty,
				HttpPostAttribute post => post.Template ?? string.Empty,
				HttpPutAttribute put => put.Template ?? string.Empty,
				HttpDeleteAttribute delete => delete.Template ?? string.Empty,
				HttpPatchAttribute patch => patch.Template ?? string.Empty,
				_ => string.Empty
			};
		}

		private static string GetHttpMethod(MethodInfo method)
		{
			var httpAttribute = method.GetCustomAttributes()
				.FirstOrDefault(IsHttpMethodAttribute);

			return httpAttribute switch
			{
				HttpGetAttribute => "GET",
				HttpPostAttribute => "POST",
				HttpPutAttribute => "PUT",
				HttpDeleteAttribute => "DELETE",
				HttpPatchAttribute => "PATCH",
				_ => "UNKNOWN"
			};
		}

		private static string CombineRoutes(string controllerRoute, string actionRoute)
		{
			if (string.IsNullOrWhiteSpace(actionRoute))
			{
				return "/" + controllerRoute.Trim('/');
			}

			return "/" + controllerRoute.Trim('/') + "/" + actionRoute.Trim('/');
		}
	}
}