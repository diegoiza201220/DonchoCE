import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'fechaint',
  standalone: true // Modern Angular standalone convention
})
export class FechaIntPipe implements PipeTransform {
  transform(value: number | string | null | undefined): string {
    if (!value) return '';

    // Convert integer to a clean string of digits
    const rawStr = value.toString().replace(/\D/g, '');
    // Handle standard 8-digit formats: YYYYMMDD
    if (rawStr.length === 8) {
      const year = rawStr.slice(0, 4);
      const month = rawStr.slice(4, 6);
      const day = rawStr.slice(6, 8);
      return `${year}-${month}-${day}`;
    }
    // Fallback if the length doesn't match standard masks
    return rawStr;
  }
}