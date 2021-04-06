import { Component, Input, OnInit } from '@angular/core';
import { FileModelCharacteristics } from '../../models/models.model';
import { ReadDataService } from '../../services/read-data.service';

@Component({
	selector: 'app-characteristics',
	templateUrl: './characteristics.component.html',
	styleUrls: ['./characteristics.component.css']
})
export class CharacteristicsComponent implements OnInit {
	@Input() fileModelCharacteristics: FileModelCharacteristics = new FileModelCharacteristics();
	
	pfile: FileModelCharacteristics = new FileModelCharacteristics();
	constructor(pReadDataService:ReadDataService) {
	 }

	ngOnInit(): void {
	}
}
