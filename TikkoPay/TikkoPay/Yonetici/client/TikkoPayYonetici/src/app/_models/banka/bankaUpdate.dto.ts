export class BankaUpdateDto {
    id: string;
    baslik: string = '';

    constructor(
        id: string,
        baslik: string = ''
    ) {
        this.id = id;
        this.baslik = baslik;
    }
}