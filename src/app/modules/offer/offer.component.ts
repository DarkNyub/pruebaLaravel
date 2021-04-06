import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ReadDataService } from '../../services/read-data.service';
import { FileModelCharacteristics, FileModelOffer, FileModelPrices } from '../../models/models.model';

@Component({
	selector: 'app-offer',
	templateUrl: './offer.component.html',
	styleUrls: ['./offer.component.css']
})
export class OfferComponent implements OnInit {
	@Output() idPassOfferModel = new EventEmitter<FileModelCharacteristics>();

	idOfferModel: string;
	fileModelOffers: FileModelOffer[] = [];

	constructor(private readDataService:ReadDataService){
		this.idOfferModel = "";
		this.getInformation();
	}

	ngOnInit(): void {
	}
	getInformation(){
		this.readDataService.getFileData().subscribe((data:any)=>{
			this.fileModelOffers = data;
			console.log(data);
		});
	}
	getOffer(){
		this.idPassOfferModel.emit(this.fileModelOffers.find(d => d.id == this.idOfferModel)?.versions[0]);
	}
}
