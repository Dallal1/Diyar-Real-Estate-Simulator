using UnityEngine;

namespace Diyar.Game
{
    public sealed class PlayerWallet : MonoBehaviour
    {
        [SerializeField, Min(0)] private int balance = 100;

        public int Balance => balance;

        private void Awake()
        {
            balance = Mathf.Max(0, balance);
        }

        public bool AddMoney(int amount)
        {
            // Reject invalid amounts and prevent integer overflow.
            if (amount <= 0 || amount > int.MaxValue - balance)
            {
                return false;
            }

            balance += amount;
            return true;
        }

        public bool TrySpendMoney(int amount)
        {
            if (amount <= 0 || amount > balance)
            {
                return false;
            }

            balance -= amount;
            return true;
        }

        [ContextMenu("Test/Add 50")]
        private void TestAddMoney()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Debug.Log($"Add 50: {AddMoney(50)}, Balance: {Balance}");
        }

        [ContextMenu("Test/Spend 40")]
        private void TestSpendMoney()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Debug.Log($"Spend 40: {TrySpendMoney(40)}, Balance: {Balance}");
        }

        [ContextMenu("Test/Spend 200")]
        private void TestInsufficientBalance()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Debug.Log($"Spend 200: {TrySpendMoney(200)}, Balance: {Balance}");
        }

        [ContextMenu("Test/Invalid Amounts")]
        private void TestInvalidAmounts()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Debug.Log($"Add 0: {AddMoney(0)}");
            Debug.Log($"Add -10: {AddMoney(-10)}");
            Debug.Log($"Spend 0: {TrySpendMoney(0)}");
            Debug.Log($"Spend -10: {TrySpendMoney(-10)}");
            Debug.Log($"Balance: {Balance}");
        }
    }
}
