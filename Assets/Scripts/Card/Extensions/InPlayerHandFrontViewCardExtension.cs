namespace Game.Cards
{
    public class InPlayerHandFrontViewCardExtension : CardExtensionComponent
    {
        private void OnMouseEnter()
        {
            if(!Active)
                return;
            
            BringViewToFront();
        }

        private void OnMouseExit()
        {
            if(!Active || Card.CardView == null)
                return;
            
            BringViewToDefault();
        }

        public void BringViewToFront()
        {
            if(Card.CardView == null)
                return;
            Card.CardView.BringViewToFront();
        }

        public void BringViewToDefault()
        {
            if(Card.CardView == null)
                return;
            Card.CardView.BringViewToDefault();
        }
    }
}