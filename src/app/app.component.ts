import { Component, OnInit, EventEmitter } from '@angular/core';
import { ReadDataService } from './services/read-data.service';
import { FileModelCharacteristics, FileModelOffer, FileModelPrices } from './models/models.model';

import { Subject } from 'rxjs';


@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
	fileModelCaracterics: FileModelCharacteristics = new FileModelCharacteristics();

	constructor(){
	}
	ngOnInit(): void {

	}
	
}
