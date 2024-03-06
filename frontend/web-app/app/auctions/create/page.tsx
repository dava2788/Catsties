import Heading from '@/app/componets/Heading'
import React from 'react'
import AuctionForm from '../AuctionForm'

export default function Create() {
  return (
    <div className=' mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg'>
      <Heading title='Sell your Car!' subtitle='Please enter the details of your car'></Heading>        
      <AuctionForm/>
    </div>
  )
}
