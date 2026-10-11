
import { Component, computed, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { Observable, Subject, Subscription, catchError, debounceTime, distinctUntilChanged, filter, finalize, map, of, switchMap, tap } from 'rxjs';
import { VehicleService } from './services/VehicleService';
import { Make } from './models/make';
import { VehicleModel } from './models/VehicleModel';
import { VehicleType } from './models/VehicleType';
import { NgSelectModule } from '@ng-select/ng-select';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, AsyncPipe, NgSelectModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly minMakeSearchLength = 2;
  protected readonly makeSearch$ = new Subject<string>();
  protected readonly makes$: Observable<Make[]>;
  protected selectedMake: Make | null = null;
  protected readonly makesError = signal('');
  protected readonly isSearchingMakes = signal(false);
  protected readonly vehicleTypes = signal<VehicleType[]>([]);
  protected readonly isLoadingVehicleTypes = signal(false);
  protected readonly vehicleTypesError = signal('');
  protected readonly showVehicleTypes = signal(false);
  private vehicleTypesSubscription?: Subscription;
  protected selectedYear: number | null = null;
  protected readonly models = signal<VehicleModel[]>([]);
  protected readonly isLoadingModels = signal(false);
  protected readonly modelsError = signal('');
  protected readonly hasSearched = signal(false);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = 12;
  protected searchedMake = '';
  protected searchedYear: number | null = null;

  protected readonly totalPages = computed(() =>
    Math.ceil(this.models().length / this.pageSize)
  );

  protected readonly paginatedModels = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.pageSize;

    return this.models().slice(startIndex, startIndex + this.pageSize);
  });

  constructor(
    private readonly vehicleService: VehicleService
  ) {
    this.makes$ = this.makeSearch$.pipe(
      map((term) => (term ?? '').trim()),
      filter((term) => term.length >= this.minMakeSearchLength),
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => {
        this.isSearchingMakes.set(true);
        this.makesError.set('');
      }),
     
      switchMap((term) =>
        this.vehicleService.searchMakes(term).pipe(
          catchError((error) => {
            console.error('API error:', error);
            this.makesError.set('Failed to load vehicle makes. Please try again.');
            return of([]);
          }),
          finalize(() => this.isSearchingMakes.set(false))
        )
      )
    );
  }

  protected readonly years: number[] = Array.from(
    { length: new Date().getFullYear() - 1980 + 1 },
    (_, index) => new Date().getFullYear() - index
  );


  protected onSelectionChange(): void {
    this.resetResults();

    if (this.selectedMake && this.selectedYear !== null) {
      this.loadModels();
    }
  }

  protected resetResults(): void {
    this.hasSearched.set(false);
    this.models.set([]);
    this.modelsError.set('');
    this.vehicleTypesSubscription?.unsubscribe();
    this.showVehicleTypes.set(false);
    this.vehicleTypes.set([]);
    this.vehicleTypesError.set('');
    this.isLoadingVehicleTypes.set(false);
  }

  protected loadVehicleTypes(makeId: number): void {
    this.vehicleTypesSubscription?.unsubscribe();
    this.showVehicleTypes.set(true);
    this.isLoadingVehicleTypes.set(true);
    this.vehicleTypesError.set('');
    this.vehicleTypes.set([]);

    this.vehicleTypesSubscription = this.vehicleService
      .getVehicleTypesForMake(makeId)
      .pipe(finalize(() => this.isLoadingVehicleTypes.set(false)))
      .subscribe({
        next: (vehicleTypes) => this.vehicleTypes.set(vehicleTypes),
        error: (error) => {
          console.error('Error loading vehicle types:', error);
          this.vehicleTypesError.set('Failed to load vehicle types. Please try again.');
        }
      });
  }

  protected loadModels(): void {
    if (!this.selectedMake || this.selectedYear === null) {
      return;
    }
    this.searchedMake = this.selectedMake.makeName;
    this.searchedYear = this.selectedYear;
    this.currentPage.set(1);
    this.isLoadingModels.set(true);
    this.modelsError.set('');
    this.models.set([]);
    this.loadVehicleTypes(this.selectedMake.id);

    this.vehicleService
      .getModelsForMakeYear(this.selectedMake.id, this.selectedYear)
      .subscribe({
        next: (models) => {
          this.models.set(models);
          this.hasSearched.set(true);
          this.isLoadingModels.set(false);
        },
        error: (error) => {
          this.isLoadingModels.set(false);
          this.modelsError.set('Failed to load models. Please try again.');
          console.error('Error loading models:', error);
        }
      });
  }
}
