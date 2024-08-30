'use client'

import { useAuctionStore } from '@/hooks/useAuctionStore'
import { useBidStore } from '@/hooks/useBidStore'
import { Auction, AuctionFinished, Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { User } from 'next-auth';
import { useParams } from 'next/navigation'
import React, { ReactNode, useCallback, useEffect, useRef } from 'react'
import AuctionCreatedToast from '../componets/AuctionCreatedToast';
import toast from 'react-hot-toast';
import { getDetailsViewData } from '../actions/auctionActions';
import AuctionFinishedToast from '../componets/AuctionFinishedToast';

type Props ={
    children : ReactNode
    user: User | null
}

export default function SignalRProvider({children, user} : Props) {
    const connection = useRef<HubConnection | null>(null);
    const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);
    const addBid = useBidStore(state => state.addBids);
    const params = useParams<{id: string}>();

    const handleAuctionFinished = useCallback((finishedAuction: AuctionFinished) => {
        const auction = getDetailsViewData(finishedAuction.auctionId);
        return toast.promise(auction, {
            loading: 'Loading',
            success: (auction) => 
                <AuctionFinishedToast 
                    finishedAuction={finishedAuction} 
                    auction={auction}
                />,
            error: (err) => 'Auction finished!'
        }, {success: {duration: 10000, icon: null}})

    },[])

    const handleAuctionCreated = useCallback((auction: Auction) => {
        if(user?.username !== auction.seller) {
            return toast(<AuctionCreatedToast auction={auction}/>,{
                duration:10000
            })
        }

    }, [user?.username])

    const handleBidPlace = useCallback((bid :Bid) => {
        if(bid.bidStatus.includes('Accepted')){
            setCurrentPrice(bid.auctionId,bid.amount)
        }

        if(params.id === bid.auctionId){
            addBid(bid);
        }

    },[setCurrentPrice, addBid, params.id]);

    useEffect(() => {
        if(!connection.current){
            connection.current  = new HubConnectionBuilder()
                                    .withUrl('http://localhost:6001/notifications')
                                    .withAutomaticReconnect()
                                    .build();

            connection.current.start()
            .then(() => 'Connected to notificationhub')
            .catch(err => console.log(err));
        }

        connection.current.on('BidPlaced', handleBidPlace);
        connection.current.on('AuctionCreated', handleAuctionCreated);
        connection.current.on('AuctionFinished', handleAuctionFinished);
        
        

        return () => {
            connection.current?.off('BidPlaced', handleBidPlace);
            connection.current?.off('AuctionCreated', handleAuctionCreated);
            connection.current?.off('AuctionFinished', handleAuctionFinished);
        }

    }, [setCurrentPrice, handleBidPlace,handleAuctionCreated,handleAuctionFinished])


    return (
        children
    )
}
