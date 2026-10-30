import { Pipe, PipeTransform } from '@angular/core';
import { KullaniciYetkileriListDto } from '../_models/kullaniciYetkileri/kullaniciYetkileriList.dto';

@Pipe({
    name: 'kullaniciYetkileriFilter',
})
export class KullaniciYetkileriFilterPipe implements PipeTransform {

    transform(items: KullaniciYetkileriListDto[], ustId: string): KullaniciYetkileriListDto[] {

        if (!items) {
            return [];
        }

        if (ustId) {
            return items.filter(k => k.ustId === ustId).sort((a, b) => a.siraNumarasi - b.siraNumarasi);
        } else {
            return items.filter(k => k.ustId === null).sort((a, b) => a.siraNumarasi - b.siraNumarasi);
        }
    }
}