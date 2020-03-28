#include "formatcontext.h"
#include "stream.h"

namespace av
{

Stream::Stream(const std::shared_ptr<char> &monitor, AVStream *st, Direction direction)
    : FFWrapperPtr<AVStream>(st),
      m_parentMonitor(monitor),
      m_direction(direction)
{
}

bool Stream::isValid() const
{
    return (!m_parentMonitor.expired() && !isNull());
}

int Stream::index() const
{
    return RAW_GET2(isValid(), index, -1);
}

int Stream::id() const
{
    return RAW_GET2(isValid(), id, -1);
}

Rational Stream::frameRate() const
{
    return RAW_GET2(isValid(), r_frame_rate, AVRational{});
}

Rational Stream::timeBase() const
{
    return RAW_GET2(isValid(), time_base, AVRational{});
}

Rational Stream::sampleAspectRatio() const
{
    return RAW_GET2(isValid(), sample_aspect_ratio, AVRational{});
}

Rational Stream::averageFrameRate() const
{
    return RAW_GET2(isValid(), avg_frame_rate, AVRational{});
}

Timestamp Stream::startTime() const
{
    return {RAW_GET2(isValid(), start_time, av::NoPts), timeBase()};
}

Timestamp Stream::duration() const
{
    return {RAW_GET2(isValid(), duration, av::NoPts), timeBase()};
}

Timestamp Stream::currentDts() const
{
    return {RAW_GET2(isValid(), cur_dts, av::NoPts), timeBase()};
}

AVMediaType Stream::mediaType() const
{
#if !USE_CODECPAR
    FF_DISABLE_DEPRECATION_WARNINGS
    return RAW_GET2(isValid() && m_raw->codec, codec->codec_type, AVMEDIA_TYPE_UNKNOWN);
    FF_ENABLE_DEPRECATION_WARNINGS
#else
    return RAW_GET2(isValid() && m_raw->codecpar, codecpar->codec_type, AVMEDIA_TYPE_UNKNOWN);
#endif
}

bool Stream::isAudio() const
{
    return (mediaType() == AVMEDIA_TYPE_AUDIO);
}

bool Stream::isVideo() const
{
    return (mediaType() == AVMEDIA_TYPE_VIDEO);
}

bool Stream::isData() const
{
    return (mediaType() == AVMEDIA_TYPE_DATA);
}

bool Stream::isSubtitle() const
{
    return (mediaType() == AVMEDIA_TYPE_SUBTITLE);
}

bool Stream::isAttachment() const
{
    return (mediaType() == AVMEDIA_TYPE_ATTACHMENT);
}

void Stream::setTimeBase(const Rational &timeBase)
{
    RAW_SET2(isValid(), time_base, timeBase.getValue());
}

void Stream::setFrameRate(const Rational &frameRate)
{
    RAW_SET2(isValid(), r_frame_rate, frameRate.getValue());
}

void Stream::setSampleAspectRatio(const Rational &aspectRatio)
{
    RAW_SET2(isValid(), sample_aspect_ratio, aspectRatio.getValue());
}

void Stream::setAverageFrameRate(const Rational &frameRate)
{
    RAW_SET2(isValid(), avg_frame_rate, frameRate.getValue());
}

int Stream::eventFlags() const noexcept
{
    if (!isValid() || m_direction != Direction::Decoding)
        return 0;
    return m_raw->event_flags;
}

bool Stream::eventFlags(int flags) const noexcept
{
    if (!isValid() || m_direction != Direction::Decoding)
        return false;
    return m_raw->event_flags & flags;
}

void Stream::eventFlagsClear(int flags) noexcept
{
    if (!isValid() || m_direction != Direction::Decoding)
        return;
    m_raw->event_flags &= ~flags;
}

} // ::av

