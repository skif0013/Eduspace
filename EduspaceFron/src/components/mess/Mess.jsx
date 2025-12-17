import React from 'react'
import { MessageCircle } from 'lucide-react'

function Mess() {
  return (
    <div className='m-auto mt-29' >
      <div className='flex items-center justify-center gap-8'>
        <div>
          <MessageCircle size={240} className='text-white' />
        </div>
       <div className="max-w-[820px] flex flex-col gap-4">
          <h2 className="text-3xl text-white font-semibold">
            Messages & Responses
          </h2>

          <p className="text-base text-gray-300">
            Communicate with the platform quickly and conveniently using the built-in messaging system.
            Ask questions, receive instant responses, and stay informed without leaving the service.
          </p>

          <p className="text-base text-gray-300">
            Our system is designed for clarity and speed: all messages are structured, easy to read,
            and available at any time. Whether you need support, explanations, or updates — everything
            is in one place.
          </p>

        </div>


      </div>
    </div>
  )
}

export default Mess
