export class TedarikcilerSelectListWithBakiyeDto {
    id: string;
    unvan: string = '';
    anlikBakiye: number;

    constructor(
        id: string,
        unvan: string,
        anlikBakiye: number
    ) {
        this.id = id;
        this.unvan = unvan;
        this.anlikBakiye = anlikBakiye;
    }
}