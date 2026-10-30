export interface PaymentMethodsDto {
    id: number;
    baslik: string;
    paymentId: number;
    depositCallbackUrl: string;
    withdrawCallbackUrl: string;
}