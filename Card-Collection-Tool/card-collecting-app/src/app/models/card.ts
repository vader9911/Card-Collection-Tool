export interface Card {
  cardID: string;
  name: string;
  manaCost?: string;
  typeLine?: string;
  oracleText?: string;
  setName?: string;
  artist?: string;
  rarity?: string;
  imageUri?: string;
  power?: string; 
  toughness?: string;  
  flavorText?: string;  
  releaseDate?: string;
  variation?: boolean;
  colors?: string[];
  colorIdentity?: string[];
  usd?: string;  // The price is in string form
  quantity: number;
}
