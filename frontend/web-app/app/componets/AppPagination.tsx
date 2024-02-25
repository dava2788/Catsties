'use client'

import { Pagination } from 'flowbite-react'
import React, { useState } from 'react'

type Props ={
    pageCount: number,
    currentPage: number,
    pageChanged: (page: number) => void
}

export default function AppPagination({pageCount, currentPage,pageChanged}:Props) {

  return (
    <Pagination 
    currentPage={currentPage} 
    onPageChange={evt => pageChanged(evt)} 
      totalPages={pageCount} 
      layout='pagination'
      showIcons={true} 
      className='text-blue-500 mb-5'
      />
  )
}
