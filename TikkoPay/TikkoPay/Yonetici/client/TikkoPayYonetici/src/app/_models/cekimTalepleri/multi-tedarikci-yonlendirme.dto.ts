export interface MultiTedarikciYonlendirmeDto {
    cekimTalepleriId: string; 
    tedarikciWithAmount: TedarikciWithAmount[];
  }
  
  export interface TedarikciWithAmount {
    tedarikciId: string; 
    amount: number;
  }