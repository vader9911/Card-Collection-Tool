import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'groupBy',
  standalone: true,
})
export class GroupByPipe implements PipeTransform {
  transform(collection: any[], property: string): any[] {
    if (!collection) return [];

    const groupedCollection = collection.reduce((acc, item) => {
      const key = item[property];
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push(item);
      return acc;
    }, {});

    return Object.keys(groupedCollection).map(key => ({ key, values: groupedCollection[key] }));
  }
}
