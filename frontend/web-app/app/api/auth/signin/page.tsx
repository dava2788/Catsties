import EmptyFilter from '@/app/componets/EmptyFilter'
import React from 'react'

export default function Page({searchParams}: {searchParams: {callbackUrl: string}}) {
  return (
    <EmptyFilter
    title='You need to log In to do That'
    subtitle='Please click below to sign in'
    showLogin
    callbackUrl={searchParams.callbackUrl}
    />
  )
}
