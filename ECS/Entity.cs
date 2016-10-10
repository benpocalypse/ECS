﻿using System;
using System.Collections.Generic;

namespace ECS
{
	// is the facade for different components, a bag for data and simple operations
    public class Entity
    {
        public List<IComponent> components
        {
            private set;
            get;
        }

        public Entity()
        {
            EntityMatcher.SubscribeEntity(this);
            components = new List<IComponent>();
        }

        ~Entity()
        {
            EntityMatcher.UnsubscribeEntity(this);
            RemoveAllComponent();
            components = null;
        }

		// Add new component to the entity, can support doubles of components
        public void AddComponent(IComponent newComponent, bool notifySystems = true)
        {
            if (newComponent == null)
            {
                Console.Write("Component that you intented to add is null, method will return void");
                return;
            }

            components.Add(newComponent);
            newComponent.entity = this;

            if (notifySystems)
            {
				// Notifies systems so they can perfom operations, like manipulating componet data
                SystemMatcher.NotifySystems();
            }
        }

		// Will replace component if there is a match, if not it will add it as a new component
        public void ReplaceComponent(IComponent replaceComponent, bool notifySystem = true)
        {
            if (replaceComponent == null)
            {
                Console.Write("Component that you intented to replace is null, method will return void");
                return;
            }

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType().Equals(replaceComponent.GetType()))
                {
                    components[i].entity = null;
                    components[i] = null;
                    components[i] = replaceComponent;
                    return;
                }
            }

            Console.Write("No match for the component, will be added as a new component to the entity");
            AddComponent(replaceComponent, notifySystem);
        }

        public void RemoveComponent<T>() where T : class, IComponent
        {
            foreach (IComponent cmp in components)
            {
                if (cmp is T)
                {
                    cmp.entity = null;
                    components.Remove(cmp);
                }
            }
        }

        public void RemoveAllComponent()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].entity = null;
                components.RemoveAt(i);
            }

            components.Clear();
        }

		public List<T> GetComponents<T>() where T : class, IComponent
		{
			List<T> requestedComponents = new List<T>();
			
			foreach (IComponent cmp in components)
			{
				if (cmp is T)
				{
					requestedComponents.Add((T)cmp);
				}
			}
			
			return requestedComponents;
		}
		
		public bool HasComponent<T>() where T : class, IComponent
		{
			foreach (IComponent cmp in components)
			{
				if (cmp is T)
				{
					return true;
				}
			}
			
			return false;
		}

		// Same hierarchy of sublclasses can have different matchers
        public List<IComponent> GetComponentsWithType(Type request)
        {
            List<IComponent> requestedComponents = new List<IComponent>();

            foreach (IComponent cmp in components)
            {
                if (cmp.GetType().Equals(request))
                {
                    requestedComponents.Add(cmp);
                }
            }

            return requestedComponents;
        }

        public bool HasAllMatchers(params Type[] matchers)
        {
            int matchedComponents = 0;

            for (int i = 0; i < matchers.Length; i++)
            {
                foreach (IComponent cmp in components)
                {
                    if (cmp.GetType().Equals(matchers[i]))
                    {
                        matchedComponents++;
                        break;
                    }
                }
            }

            return matchedComponents == matchers.Length && matchedComponents != 0;
        }

        public bool HasAnyMatcher(params Type[] matchers)
        {
            for (int i = 0; i < matchers.Length; i++)
            {
                foreach (IComponent cmp in components)
                {
                    if (cmp.GetType().Equals(matchers[i]))
                        return true;
                }
            }

            return false;
        }

        public void RemoveComponent(Type type)
		{
			foreach (IComponent cmp in components)
			{
                if (cmp.GetType().Equals(type))
				{
					components.Remove(cmp);
				}
			}
		}
    }
}