import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'numberRange',
})
export class NumberRangePipe implements PipeTransform {

    transform(end: number): number[] {
        return Array.from({ length: end }, (_, index) => index + 1);
    }
}