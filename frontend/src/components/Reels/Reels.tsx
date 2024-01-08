import React, { useRef, useEffect } from 'react';
import { useInView } from 'react-intersection-observer';
import './Reels.css';
import { API_URL } from '../../config';


interface ReelsProps {
  videoId: string; // Assuming videoId is a string, adjust the type accordingly
}

const Reels: React.FC<ReelsProps> = ({ videoId }) => {
  const vidRef = useRef<HTMLVideoElement | null>(null);
  const [ref, inView] = useInView({
    threshold: 0.5,
  });

  // Build the video URL dynamically based on the videoId
  const videoUrl = `${API_URL}/Reel/${videoId}`;

  useEffect(() => {
    if (inView) {
      vidRef.current?.play();
    } else {
      vidRef.current?.pause();
    }
  }, [inView]);

  return (
    <div ref={ref} className="reel-card">
      <video
        className="reel-player"
        ref={vidRef}
        src={videoUrl}
        loop
        muted={false}
        controls
        preload="metadata"
      ></video>
    </div>
  );
};

export default Reels;
