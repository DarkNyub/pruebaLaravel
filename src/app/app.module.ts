import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReadDataService } from './services/read-data.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
//SERVICES
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { OfferComponent } from './modules/offer/offer.component';
import { CharacteristicsComponent } from './modules/characteristics/characteristics.component';
import { PricesComponent } from './modules/prices/prices.component';

@NgModule({
  declarations: [
    AppComponent,
    OfferComponent,
    CharacteristicsComponent,
    PricesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [ReadDataService],
  bootstrap: [AppComponent]
})
export class AppModule { }
