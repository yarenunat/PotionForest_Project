using UnityEngine;
using System;
using System.Collections.Generic;

namespace PotionForest.Core
{
    /// <summary>
    /// Manages ingredient inventory and triggers events for cozy UI feedback when collecting items.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        // Stores ingredients like "glowing_mushroom", "starflower", "moonlight_dew"
        private Dictionary<string, int> ingredients = new Dictionary<string, int>();

        // Event triggered when an item is collected. UI can listen to this to play a 'pop' animation.
        public event Action<string, int> OnItemCollected;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Adds an ingredient to the inventory and triggers the collection event.
        /// </summary>
        /// <param name="ingredientId">The string identifier of the ingredient.</param>
        /// <param name="amount">The amount to add.</param>
        public void AddIngredient(string ingredientId, int amount = 1)
        {
            if (ingredients.ContainsKey(ingredientId))
            {
                ingredients[ingredientId] += amount;
            }
            else
            {
                ingredients.Add(ingredientId, amount);
            }

            // Trigger the soft 'pop' event so the UI can react gently
            OnItemCollected?.Invoke(ingredientId, amount);
            
            Debug.Log($"Collected {amount} of {ingredientId}. Total: {ingredients[ingredientId]}");
        }

        /// <summary>
        /// Checks if the inventory contains at least the required amount of an ingredient.
        /// </summary>
        public bool HasIngredient(string ingredientId, int amount)
        {
            if (ingredients.TryGetValue(ingredientId, out int currentAmount))
            {
                return currentAmount >= amount;
            }
            return false;
        }

        /// <summary>
        /// Consumes an ingredient if available.
        /// </summary>
        public void ConsumeIngredient(string ingredientId, int amount)
        {
            if (HasIngredient(ingredientId, amount))
            {
                ingredients[ingredientId] -= amount;
            }
            else
            {
                Debug.LogWarning($"Not enough {ingredientId} to consume!");
            }
        }
    }
}
