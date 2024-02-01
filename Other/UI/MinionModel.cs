using Fizz6.UI;

namespace Fizz6.Roguelike.UI
{
    public class MinionModel : Model
    {
        private Minion.Minion minion;

        private Minion.Minion Minion
        {
            get => minion;
            set
            {
                if (minion != null)
                {
                    minion.DamageEvent -= OnDamage;
                    minion.FaintEvent -= OnFaint;
                }

                minion = value;
                
                if (minion != null)
                {
                    minion.DamageEvent += OnDamage;
                    minion.FaintEvent += OnFaint;
                }
            }
        }
        
        public void Initialize(Minion.Minion minion)
        {
            Minion = minion;
            Broadcast();
        }

        protected override void OnDestroy()
        {
            Minion = null;
        }

        private void OnDamage(int damage) => Broadcast();
        private void OnFaint() => Broadcast();
    }
}