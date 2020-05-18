// MIT License
// 
// Copyright (c) 2020 Alexey Politov
// https://github.com/EmptyBucket/Modules
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRootModule<T>(
		this IServiceCollection serviceCollection, IConfiguration configuration)
		where T : IModule
	{
		IModule Factory(Type type)
		{
			return (IModule)Activator.CreateInstance(type)!;
		}

		foreach (var module in KvTarjan(typeof(T), Factory, m => m.Dependencies))
			module.ConfigureServices(serviceCollection, configuration);

		return serviceCollection;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	internal static IEnumerable<TValue> KvTarjan<TKey, TValue>(
		TKey rootKey, Func<TKey, TValue> map, Func<TValue, IEnumerable<TKey>> next) where TKey : notnull
	{
		IEnumerable<TValue> DfsPostOrder(TKey key, IDictionary<TKey, bool> marks)
		{
			if (!marks.TryAdd(key, false))
				if (marks[key]) yield break;
				else throw new Exception("Cycle detected");

			var value = map(key);

			foreach (var childKey in next(value))
			foreach (var childValue in DfsPostOrder(childKey, marks))
				yield return childValue;

			marks[key] = true;
			yield return value;
		}

		var sorted = DfsPostOrder(rootKey, new Dictionary<TKey, bool>());
		return sorted;
	}
}