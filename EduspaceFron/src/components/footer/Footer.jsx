import React from 'react'

function Footer() {
  return (
    <footer className="m-auto mt-28 px-8 py-14">
      <div className="flex flex-wrap gap-12 justify-between">

        <div className="max-w-[260px]">
          <h2 className="text-white text-3xl font-bold mb-4">
            Eduspace
          </h2>
          <p className="text-gray-400 text-sm mb-2">
            Eduspace is a modern learning platform that helps students
            gain knowledge faster using smart tools and AI assistance.
          </p>
          <p className="text-gray-400 text-sm">
            Learn. Practice. Grow — all in one place.
          </p>
        </div>


        <div>
          <h3 className="text-white font-semibold mb-4">
            Product
          </h3>
          <ul className="flex flex-col gap-2 text-gray-400 text-sm">
            <li>Courses</li>
            <li>AI Assistant</li>
            <li>Messages</li>
            <li>Pricing</li>
          </ul>
        </div>

        <div>
          <h3 className="text-white font-semibold mb-4">
            Company
          </h3>
          <ul className="flex flex-col gap-2 text-gray-400 text-sm">
            <li>About us</li>
            <li>Careers</li>
            <li>Partners</li>
            <li>Blog</li>
          </ul>
        </div>

        <div>
          <h3 className="text-white font-semibold mb-4">
            Support
          </h3>
          <ul className="flex flex-col gap-2 text-gray-400 text-sm">
            <li>Help center</li>
            <li>Contact</li>
            <li>Privacy policy</li>
            <li>Terms of service</li>
          </ul>
        </div>

      </div>

      <div className="mt-12 border-t border-white/10 pt-6 flex flex-col md:flex-row items-center justify-between gap-4">
        <p className="text-gray-500 text-sm">
          © {new Date().getFullYear()} Eduspace. All rights reserved.
        </p>
        <p className="text-gray-500 text-sm">
          Built for students and educators worldwide.
        </p>
      </div>
    </footer>
  )
}

export default Footer
