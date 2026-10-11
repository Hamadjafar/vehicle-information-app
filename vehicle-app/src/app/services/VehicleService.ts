
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Make } from '../models/make';
import { VehicleModel } from '../models/VehicleModel';
import { VehicleType } from '../models/VehicleType';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly apiUrl = 'http://localhost:5000/api/Makes';

  constructor(private readonly http: HttpClient) { }

  searchMakes(search: string, take = 20): Observable<Make[]> {
    return this.http.get<Make[]>(this.apiUrl, {
      params: { search, take: take.toString() }
    });
  }


  getVehicleTypesForMake(makeId: number): Observable<VehicleType[]> {
    return this.http.get<VehicleType[]>(`${this.apiUrl}/${makeId}/VehicleTypes`);
  }

  getModelsForMakeYear(
    makeId: number,
    year: number
  ): Observable<VehicleModel[]> {
    return this.http.get<VehicleModel[]>(`${this.apiUrl}/${makeId}/Models`,
      {
        params: { year: year.toString() }
      }
    );
  }

}
