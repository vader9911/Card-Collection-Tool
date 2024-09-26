import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'titlecaseKey',
  standalone: true,
})
export class TitlecaseKeyPipe implements PipeTransform {
  transform(value: unknown): string {
    if (typeof value !== 'string') {
      return 'Unknown'; 
    }

    return value
      .split(/(?=[A-Z])/)
      .join(' ')
      .replace(/\b\w/g, (firstLetter) => firstLetter.toUpperCase());
  }
}
